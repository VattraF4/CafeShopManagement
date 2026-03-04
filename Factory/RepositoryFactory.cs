using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Repository;
using OOADCafeShopManagement.Repositories;

namespace OOADCafeShopManagement.Factory
{
    /// <summary>
    /// Factory Pattern for creating Repository instances
    /// Provides centralized repository creation and dependency management
    /// Implements Singleton pattern for repository instances
    /// </summary>
    public class RepositoryFactory
    {
        private static RepositoryFactory _instance;
        private static readonly object _lock = new object();

        // Repository instances (cached for reuse)
        private IUserRepository _userRepository;
        private IProductRepository _productRepository;
        private IOrderRepository _orderRepository;

        // Private constructor for Singleton
        private RepositoryFactory() { }

        /// <summary>
        /// Gets the singleton instance of RepositoryFactory
        /// </summary>
        public static RepositoryFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RepositoryFactory();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Creates or returns cached UserRepository instance
        /// </summary>
        public IUserRepository CreateUserRepository()
        {
            if (_userRepository == null)
            {
                lock (_lock)
                {
                    if (_userRepository == null)
                    {
                        _userRepository = new UserRepository();
                    }
                }
            }
            return _userRepository;
        }

        /// <summary>
        /// Creates or returns cached ProductRepository instance
        /// </summary>
        public IProductRepository CreateProductRepository()
        {
            if (_productRepository == null)
            {
                lock (_lock)
                {
                    if (_productRepository == null)
                    {
                        _productRepository = new ProductRepository();
                    }
                }
            }
            return _productRepository;
        }

        /// <summary>
        /// Creates or returns cached OrderRepository instance
        /// </summary>
        public IOrderRepository CreateOrderRepository()
        {
            if (_orderRepository == null)
            {
                lock (_lock)
                {
                    if (_orderRepository == null)
                    {
                        _orderRepository = new OrderRepository();
                    }
                }
            }
            return _orderRepository;
        }

        /// <summary>
        /// Resets all cached repositories (useful for testing or refresh scenarios)
        /// </summary>
        public void ResetRepositories()
        {
            lock (_lock)
            {
                _userRepository = null;
                _productRepository = null;
                _orderRepository = null;
            }
        }

        /// <summary>
        /// Creates a new instance of UserRepository (non-cached)
        /// </summary>
        public IUserRepository CreateNewUserRepository()
        {
            return new UserRepository();
        }

        /// <summary>
        /// Creates a new instance of ProductRepository (non-cached)
        /// </summary>
        public IProductRepository CreateNewProductRepository()
        {
            return new ProductRepository();
        }

        /// <summary>
        /// Creates a new instance of OrderRepository (non-cached)
        /// </summary>
        public IOrderRepository CreateNewOrderRepository()
        {
            return new OrderRepository();
        }
    }
}
