namespace ClienteService.Domain.Interfaces;

public interface IClienteEventPublisher
{
    Task PublishClienteCreatedAsync(int clienteId, string nombre);
    Task PublishClienteUpdatedAsync(int clienteId, string nombre);
    Task PublishClienteDeletedAsync(int clienteId);
}
