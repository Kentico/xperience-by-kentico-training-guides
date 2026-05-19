using CMS.Commerce;
using CMS.Scheduler;
using TrainingGuides.Web.Features.Commerce.PriceCalculation.Models;
using TrainingGuides.Web.Features.Commerce.ScheduledTasks;

[assembly: RegisterScheduledTask(identifier: DummyOrderCreationScheduledTask.IDENTIFIER, typeof(DummyOrderCreationScheduledTask))]

namespace TrainingGuides.Web.Features.Commerce.ScheduledTasks;

public class DummyOrderCreationScheduledTask(
    IOrderCreationService<OrderData, PriceCalculationRequest, TrainingGuidesPriceCalculationResult, AddressDto> orderCreationService) : IScheduledTask
{
    public const string IDENTIFIER = "TrainingGuides.DummyOrderCreationScheduledTask";

    private const int ORDER_COUNT = 5;

    private static readonly int[] productIdentifiers =
        [93, 94, 98, 130, 131, 19, 20, 21, 26, 118, 119, 120, 122, 125];

    public async Task<ScheduledTaskExecutionResult> Execute(ScheduledTaskConfigurationInfo task, CancellationToken cancellationToken)
    {
        var random = new Random();

        for (int i = 0; i < ORDER_COUNT; i++)
        {
            // Pick 1-3 random products for each order
            int itemCount = random.Next(1, 4);
            var selectedProducts = productIdentifiers
                .OrderBy(_ => random.Next())
                .Take(itemCount)
                .Select(id => new OrderItem
                {
                    ProductIdentifier = new TrainingGuidesPriceIdentifier { Identifier = id },
                    Quantity = random.Next(1, 4)
                })
                .ToArray();

            var orderData = new OrderData
            {
                OrderNumber = $"DUMMY-{DateTime.Now:yyyyMMddHHmmss}-{i + 1}",
                LanguageName = "en",
                BillingAddress = new AddressDto
                {
                    FirstName = $"Test{i + 1}",
                    LastName = "User",
                    Email = $"testuser{i + 1}@localhost.local",
                    Line1 = $"{100 + i} Main St",
                    City = "Testville",
                    Zip = "10001",
                    CountryID = 271,
                    StateID = 70
                },
                OrderItems = selectedProducts,
            };

            int q = await orderCreationService.CreateOrder(orderData, cancellationToken);
        }

        return ScheduledTaskExecutionResult.Success;
    }
}
