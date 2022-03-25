using MassTransit;

namespace CRM.APILayer.Producers
{
    public interface ICRMProducers
    {
        Task NotifyAccountAdded(int id);
        Task NotifyLeadAdded(int id);
    }
}