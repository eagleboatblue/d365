namespace Nov.Caps.Int.D365.Data.Common
{
    public class ConnectionSettings
    {
        public string Host { get; }

        public string Port { get; }

        public string Database { get; }

        public string User { get; }

        public string Password { get; }

        public int ReadSize { get; }

        public int ReceiveSize { get; }

        public ConnectionSettings(string host, string port, string database, string user, string password, int readSize, int receiveSize)
        {
            this.Host = host;
            this.Port = port;
            this.Database = database;
            this.User = user;
            this.Password = password;
            this.ReadSize = readSize;
            this.ReceiveSize = receiveSize;
        }

        public override string ToString()
        {
            return $"Host={this.Host};Port={this.Port};Database={this.Database};Username={this.User};Password={this.Password};SSL Mode=Disable;Trust Server Certificate=true;Pooling=false;Server Compatibility Mode=NoTypeLoading;Timeout=0;Command Timeout=0;Read Buffer Size={this.ReadSize};Socket Receive Buffer Size={this.ReceiveSize};No Reset On Close=true";
        }
    }
}
