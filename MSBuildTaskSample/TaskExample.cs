namespace MSBuildTaskSample
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Versioning;
    using Azure.Identity;
    using Microsoft.Build.Framework;
    using Microsoft.Graph;

    public sealed class TaskExample : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// Gets or sets the Azure Active Directory tenant (directory) ID or name.
        /// </summary>
        public string TenantId { get; set; } = "YOUR-TENANT-ID";

        /// <summary>
        /// Gets or sets the team(group) identifier.
        /// </summary>
        public string TeamId { get; set; } = "YOUR-TEAM-ID";

        /// <summary>
        /// Gets or sets the channel identifier.
        /// </summary>
        public string ChannelId { get; set; } = "YOUR-CHANNEL-ID";

        /// <summary>
        /// Gets or sets the client (application) ID of an App Registration in the tenant.
        /// </summary>
        public string ClientId { get; set; } = "YOUR-CLIENT-ID";

        /// <summary>
        /// Gets or sets the user account's user name.
        /// </summary>
        public string Login { get; set; } = "YOUR-USER-LOGIN";

        /// <summary>
        /// Gets or sets the user account's password.
        /// </summary>
        public string Password { get; set; } = "YOUR-USER-PASSWORD";

        /// <inheritdoc />
        public override bool Execute()
        {
            var targetFrameworkAttribute = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
                .SingleOrDefault() as TargetFrameworkAttribute;

            Log.LogMessage(MessageImportance.High, $"OS Description: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
            Log.LogMessage(MessageImportance.High, $"Assembly TargetFramework: {targetFrameworkAttribute.FrameworkName}");
            Log.LogMessage(MessageImportance.High, $"CRL: {Environment.Version}");

            LogAssemblyVersion("Azure.Core");
            LogAssemblyVersion("Azure.Identity");
            LogAssemblyVersion("Microsoft.Graph");

            try
            {
                var credential = new UsernamePasswordCredential(Login, Password, TenantId, ClientId);

                var graphClient = new GraphServiceClient(credential);

                var teams = graphClient.Me.JoinedTeams.GetAsync().GetAwaiter().GetResult();

                Log.LogMessage($"Joined teams: {teams.Value.Count}");

            }
            catch (Exception ex)
            {
                Log.LogError($"Unexpected exception: {ex}");
                return false;
            }

            Log.LogMessage("Task completed successfully");
            return true;
        }

        private void LogAssemblyVersion(string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a
                => a.GetName().Name.Equals(assemblyName, StringComparison.Ordinal));

            if (assembly == default)
            {
                Log.LogMessage(MessageImportance.High, $"Assembly '{assembly.FullName}' not found");
            }
            else
            {
                Log.LogMessage(MessageImportance.High, $"Assembly '{assembly.FullName}' version: " +
                    System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion);
            }
        }
    }
}
