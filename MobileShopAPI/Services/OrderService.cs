// Services/OrderService.cs
using MobileShopAPI.DTOs.Order;
using MobileShopAPI.Models;
using MobileShopAPI.Repositories.Interfaces;
using MobileShopAPI.Services.Interfaces;

namespace MobileShopAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderAddressRepository _orderAddressRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductRepository productRepository,
            ICartRepository cartRepository,
            IOrderAddressRepository orderAddressRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _orderAddressRepository = orderAddressRepository;
            _logger = logger;
        }
        
        public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderDto dto)
        {
            try
            {
                // 1. Get user's cart
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any())
                    throw new InvalidOperationException("Cart is empty");

                // 2. Validate products and stock
                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Product {cartItem.ProductId} not found");

                    if (product.StockQuantity < cartItem.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for product {product.SKU}");
                }

                // 3. Generate order number
                var orderNumber = GenerateOrderNumber();

                // 4. Calculate amounts
                var subtotal = cart.Items.Sum(item => item.Price * item.Quantity);
                var taxAmount = 0m;
                var shippingAmount = 750;
                var totalAmount = subtotal + taxAmount + shippingAmount;

                // 5. Create order (WITHOUT address properties - they're separate)
                var order = new Order
                {
                    OrderNumber = orderNumber,
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = Enums.OrderStatus.Pending,
                    
                    // Amounts
                    Subtotal = subtotal,
                    TaxAmount = taxAmount,
                    ShippingAmount = shippingAmount,
                    DiscountAmount = 0m,
                    TotalAmount = totalAmount,
                    
                    // Payment
                    PaymentType = dto.PaymentMethod, // Note: Your model uses PaymentType
                    PaymentStatus = Enums.PaymentStatus.Pending,
                    
                    // Notes
                    CustomerNotes = dto.CustomerNotes,
                    
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // 6. Save order first to get ID
                await _orderRepository.CreateAsync(order);

                // 7. Create shipping address
                var shippingAddress = new OrderAddress
                {
                    OrderId = order.Id,
                    AddressType = Enums.AddressType.Shipping,
                    FullName = dto.ShippingAddress.FullName,
                    AddressLine1 = dto.ShippingAddress.AddressLine1,
                    AddressLine2 = dto.ShippingAddress.AddressLine2,
                    City = dto.ShippingAddress.City,
                    State = dto.ShippingAddress.State,
                    ZipCode = dto.ShippingAddress.ZipCode,
                    Country = dto.ShippingAddress.Country,
                    Phone = dto.ShippingAddress.Phone,
                    Email = dto.ShippingAddress.Email
                };

                await _orderAddressRepository.CreateAsync(shippingAddress);

                // 9. Create order items
                foreach (var cartItem in cart.Items)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
                    
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        PriceAtPurchase = cartItem.Price,
                        CreatedAt = DateTime.UtcNow,
                    };

                    await _orderItemRepository.CreateAsync(orderItem);
                    
                    // 10. Update product stock
                    product.StockQuantity -= cartItem.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                // 11. Clear cart
                await _cartRepository.ClearCartAsync(cart.Id);
                await _cartRepository.SaveChangesAsync();

                // 12. Return created order
                return await GetOrderByIdAsync(order.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            // Get all orders for the user
            var orders = await _orderRepository.GetByUserIdAsync(userId);
    
            // Get all order IDs
            var orderIds = orders.Select(o => o.Id).ToList();
    
            // Get all order items for these orders in one query
            var allOrderItems = await _orderItemRepository.GetByOrderIdsAsync(orderIds);
    
            // Get all addresses for these orders
            var allAddresses = await _orderAddressRepository.GetByOrderIdsAsync(orderIds);
            
            var orderDtos = new List<OrderDto>();
    
            foreach (var order in orders)
            {
                // Get items for this specific order
                var orderItems = allOrderItems.Where(oi => oi.OrderId == order.Id).ToList();
                var addresses = allAddresses.Where(a => a.OrderId == order.Id).ToList();
        
                orderDtos.Add(MapToOrderDto(order, orderItems, addresses));
            }
    
            return orderDtos;
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId, string userId) 
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
    
            if (order == null || order.UserId != userId)
                throw new KeyNotFoundException("Order not found");

            var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
            var addresses = await _orderAddressRepository.GetByOrderIdAsync(orderId);
    
            return MapToOrderDto(order, orderItems, addresses);
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto, string adminId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");

            var oldStatus = order.Status;
            order.Status = dto.Status;
            order.AdminNotes = dto.AdminNotes;
            order.TrackingNumber = dto.TrackingNumber;
            order.UpdatedAt = DateTime.UtcNow;

            // Update dates based on status
            if (dto.Status == Enums.OrderStatus.Shipped)
                order.ShippedDate = DateTime.UtcNow;
            else if (dto.Status == Enums.OrderStatus.Delivered)
                order.DeliveredDate = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            _logger.LogInformation("Order {OrderId} status changed from {OldStatus} to {NewStatus} by {AdminId}", 
                orderId, oldStatus, dto.Status, adminId);

            var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
            return MapToOrderDto(order, orderItems);
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            
            if (order == null || order.UserId != userId)
                throw new KeyNotFoundException("Order not found");

            if (order.Status != Enums.OrderStatus.Pending && order.Status != Enums.OrderStatus.Confirmed)
                throw new InvalidOperationException("Order cannot be cancelled at this stage");

            order.Status = Enums.OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            // Restore product stock
            var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<List<OrderSummaryDto>> GetAllOrdersAsync(Enums.OrderStatus? status = null)
        {
            IEnumerable<Order> orders;
            
            if (status.HasValue)
                orders = await _orderRepository.GetByStatusAsync(status.Value);
            else
                orders = await _orderRepository.GetAllAsync();

            return orders.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = 0, // We'll fix this later
                PaymentStatus = o.PaymentStatus
            }).ToList();
        }

        public async Task<OrderDto> GetOrderForAdminAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Order not found");

            var orderItems = await _orderItemRepository.GetByOrderIdAsync(orderId);
            return MapToOrderDto(order, orderItems);
        }

        // Helper methods
        private string GenerateOrderNumber()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(10000, 99999);
            return $"ORD-{datePart}-{randomPart}";
        }

        private string GetAttributeSummary(ICollection<ProductAttribute> productAttributes)
        {
            if (productAttributes == null || !productAttributes.Any())
                return string.Empty;
    
            var attributes = productAttributes.Select(pa => 
                $"{pa.AttributeValue?.AttributeType?.Name}: {pa.AttributeValue?.Value}");
    
            return string.Join(", ", attributes);
        }

        private OrderDto MapToOrderDto(Order order, IEnumerable<OrderItem> orderItems, IEnumerable<OrderAddress> addresses = null)
{
    // Get shipping and billing addresses from the provided addresses
    var shippingAddress = addresses?.FirstOrDefault(a => a.AddressType == Enums.AddressType.Shipping);
    var billingAddress = addresses?.FirstOrDefault(a => a.AddressType == Enums.AddressType.Billing);

    var orderItemDtos = orderItems.Select(oi => new OrderItemDto
    {
        Id = oi.Id,
        ProductId = oi.ProductId,
        Quantity = oi.Quantity,
        UnitPrice = oi.PriceAtPurchase,
        TotalPrice = oi.Quantity * oi.PriceAtPurchase,
        
        ProductName = oi.Product?.Model?.Name ?? "Unknown Product",
        SKU = oi.Product?.SKU ?? "N/A",
        AttributeSummary = GetAttributeSummary(oi.Product?.ProductAttributes),
        ProductImage = oi.Product?.ProductImageAssignments?
            .FirstOrDefault()?.ProductImage?.ImageUrl
    }).ToList();

    return new OrderDto
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        UserId = order.UserId,
        OrderDate = order.OrderDate,
        Status = order.Status,
        
        Subtotal = order.Subtotal,
        TaxAmount = order.TaxAmount,
        ShippingFee = order.ShippingAmount,
        DiscountAmount = order.DiscountAmount,
        TotalAmount = order.TotalAmount,
        
        PaymentMethod = order.PaymentType,
        PaymentStatus = order.PaymentStatus,
        
        ShippingAddress = shippingAddress != null ? new AddressDto
        {
            FullName = shippingAddress.FullName,
            AddressLine1 = shippingAddress.AddressLine1,
            AddressLine2 = shippingAddress.AddressLine2,
            City = shippingAddress.City,
            State = shippingAddress.State,
            ZipCode = shippingAddress.ZipCode,
            Country = shippingAddress.Country,
            Phone = shippingAddress.Phone,
            Email = shippingAddress.Email
        } : null,
        
        BillingAddress = billingAddress != null ? new AddressDto
        {
            FullName = billingAddress.FullName,
            AddressLine1 = billingAddress.AddressLine1,
            AddressLine2 = billingAddress.AddressLine2,
            City = billingAddress.City,
            State = billingAddress.State,
            ZipCode = billingAddress.ZipCode,
            Country = billingAddress.Country,
            Phone = billingAddress.Phone,
            Email = billingAddress.Email
        } : null,
        
        OrderItems = orderItemDtos,
        
        TrackingNumber = order.TrackingNumber,
        ShippedDate = order.ShippedDate,
        DeliveredDate = order.DeliveredDate,
        
        CustomerNotes = order.CustomerNotes,
        AdminNotes = order.AdminNotes,
        
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };
}
    }
}