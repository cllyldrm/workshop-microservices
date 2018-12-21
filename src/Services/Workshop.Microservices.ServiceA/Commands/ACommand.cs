using MediatR;

namespace Workshop.Microservices.ServiceA.Commands
{
    public class ACommand : IRequest<bool>
    {
        public string Information { get; private set; }

        public ACommand(string information)
        {
            Information = information;
        }
    }
}