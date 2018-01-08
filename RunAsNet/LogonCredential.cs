namespace RunAsNet
{
    public class LogonCredential
    {
        public string Domain { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Command { get; set; }

        public bool IsValid()
        {
            return (
                    !string.IsNullOrWhiteSpace(Domain) &&
                    !string.IsNullOrWhiteSpace(Username) &&
                    !string.IsNullOrWhiteSpace(Password) &&
                    !string.IsNullOrWhiteSpace(Command)
                );
        }
    }
}
