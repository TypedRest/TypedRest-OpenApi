namespace TypedRest.OpenApi.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that is invoked with no input or output.
    /// </summary>
    public class ActionEndpoint : Endpoint
    {
        public override string Type => "action";
    }
}