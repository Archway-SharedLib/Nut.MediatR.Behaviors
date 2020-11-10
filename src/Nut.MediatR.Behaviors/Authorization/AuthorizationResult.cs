namespace Nut.MediatR
{
    public sealed class AuthorizationResult
    {
        private AuthorizationResult()
        {
        }

        public string? FailurMessage { get; private set; }

        public bool Succeeded { get; private set; }

        public static AuthorizationResult Failed(string failurMessage)
        {
            return new AuthorizationResult() { Succeeded = false, FailurMessage = failurMessage };
        }

        public static AuthorizationResult Success()
        {
            return new AuthorizationResult() { Succeeded = true };
        }
    }
}
