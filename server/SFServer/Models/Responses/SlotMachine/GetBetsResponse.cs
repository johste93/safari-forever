using System;
namespace SFServer.Models.Responses.SlotMachineResponses
{
    public class GetBetsResponse
    {
        public int MinimumBet { get; set; }
        public int MaximumBet { get; set; }

        public GetBetsResponse(int minimumBet, int maximumBet)
        {
            this.MinimumBet = minimumBet;
            this.MaximumBet = maximumBet;
        }
    }
}
