using Grpc.Core;
using Microsoft.Extensions.Logging;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Otus.Teaching.PromoCodeFactory.WebHost
{
    public class CustomerGrpcService: CustomerGrpc.CustomerGrpcBase
    {
        private readonly ILogger<CustomerGrpcService> _logger;
        private readonly IRepository<Customer> _customerRepository;

        public CustomerGrpcService(ILogger<CustomerGrpcService> logger, IRepository<Customer> customerRepository) 
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public override async Task<CustomerList> GetCustomers(Empty request, ServerCallContext context)
        {
            var customers = await _customerRepository.GetAllAsync();
            var customersReply = customers.Select(x => new CustomerShortResponseReply
            {
                Id = x.Id.ToString(),
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            });

            var customerList = new CustomerList();
            customerList.Customers.AddRange(customersReply);

            return customerList;
        }

        public override async Task<CustomerShortResponseReply> GetCustomer(GuidRequest request, ServerCallContext context)
        {
            var guid = Guid.Parse(request.Guid);
            var customer = await _customerRepository.GetByIdAsync(guid);
            var customerShort = new CustomerShortResponseReply();
            
            if(customer != null)
            {
                customerShort = new()
                {
                    Id = customer.Id.ToString(),
                    Email = customer.Email,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName
                };
            }           

            return customerShort;
        }
    }
}
