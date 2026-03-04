using OOADCafeShopManagement.Services;

namespace OOADCafeShopManagement.Factory
{
    /// <summary>
    /// Factory Pattern for creating Service instances
    /// Provides centralized service creation with proper dependency injection
    /// Implements Singleton pattern for service instances
    /// </summary>
    public class ServiceFactory
    {
        private static ServiceFactory _instance;
        private static readonly object _lock = new object();
        private readonly RepositoryFactory _repositoryFactory;

        // Service instances (cached for reuse)
        private UserService _userService;
        private ProductService _productService;
        private OrderService _orderService;

        // Private constructor for Singleton
        private ServiceFactory()
        {
            _repositoryFactory = RepositoryFactory.Instance;
        }

        /// <summary>
        /// Gets the singleton instance of ServiceFactory
        /// </summary>
        public static ServiceFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ServiceFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Creates or returns cached UserService instance
        /// </summary>
        public UserService CreateUserService()
        {
            if (_userService == null)
            {
                lock (_lock)
                {
                    if (_userService == null)
                    {
                        var userRepository = _repositoryFactory.CreateUserRepository();
                        _userService = new UserService(userRepository);
                    }
                }
            }
            return _userService;
        }

        /// <summary>
        /// Creates or returns cached ProductService instance
        /// </summary>
        public ProductService CreateProductService()
        {
            if (_productService == null)
            {
                lock (_lock)
                {
                    if (_productService == null)
                    {
                        var productRepository = _repositoryFactory.CreateProductRepository();
                        _productService = new ProductService(productRepository);
                    }
                }
            }
            return _productService;
        }

        /// <summary>
        /// Creates or returns cached OrderService instance
        /// </summary>
        public OrderService CreateOrderService()
        {
            if (_orderService == null)
            {
                lock (_lock)
                {
                    if (_orderService == null)
                    {
                        var orderRepository = _repositoryFactory.CreateOrderRepository();
                        _orderService = new OrderService(orderRepository);
                    }
                }
            }
            return _orderService;
        }

        /// <summary>
        /// Resets all cached services (useful for testing or refresh scenarios)
        /// </summary>
        public void ResetServices()
        {
            lock (_lock)
            {
                _userService = null;
                _productService = null;
                _orderService = null;
            }
        }

        /// <summary>
        /// Creates a new instance of UserService (non-cached)
        /// </summary>
        public UserService CreateNewUserService()
        {
            var userRepository = _repositoryFactory.CreateNewUserRepository();
            return new UserService(userRepository);
        }

        /// <summary>
        /// Creates a new instance of ProductService (non-cached)
        /// </summary>
        public ProductService CreateNewProductService()
        {
            var productRepository = _repositoryFactory.CreateNewProductRepository();
            return new ProductService(productRepository);
        }

        /// <summary>
        /// Creates a new instance of OrderService (non-cached)
        /// </summary>
        public OrderService CreateNewOrderService()
        {
            var orderRepository = _repositoryFactory.CreateNewOrderRepository();
            return new OrderService(orderRepository);
        }
    }
}
