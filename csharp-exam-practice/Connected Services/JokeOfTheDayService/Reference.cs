﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     //
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JokeOfTheDayService
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="JokeOfTheDayService.IJokeOfTheDayService")]
    public interface IJokeOfTheDayService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IJokeOfTheDayService/GetJoke", ReplyAction="http://tempuri.org/IJokeOfTheDayService/GetJokeResponse")]
        System.Threading.Tasks.Task<string> GetJokeAsync(int jokeStrength);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface IJokeOfTheDayServiceChannel : JokeOfTheDayService.IJokeOfTheDayService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class JokeOfTheDayServiceClient : System.ServiceModel.ClientBase<JokeOfTheDayService.IJokeOfTheDayService>, JokeOfTheDayService.IJokeOfTheDayService
    {
        
    /// <summary>
    /// Implement this partial method to configure the service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The endpoint to configure</param>
    /// <param name="clientCredentials">The client credentials</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public JokeOfTheDayServiceClient() : 
                base(JokeOfTheDayServiceClient.GetDefaultBinding(), JokeOfTheDayServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IJokeOfTheDayService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public JokeOfTheDayServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(JokeOfTheDayServiceClient.GetBindingForEndpoint(endpointConfiguration), JokeOfTheDayServiceClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public JokeOfTheDayServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(JokeOfTheDayServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public JokeOfTheDayServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(JokeOfTheDayServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public JokeOfTheDayServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<string> GetJokeAsync(int jokeStrength)
        {
            return base.Channel.GetJokeAsync(jokeStrength);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IJokeOfTheDayService))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IJokeOfTheDayService))
            {
                return new System.ServiceModel.EndpointAddress("http://localhost:22645/JokeOfTheDayService.svc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return JokeOfTheDayServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_IJokeOfTheDayService);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return JokeOfTheDayServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_IJokeOfTheDayService);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding_IJokeOfTheDayService,
        }
    }
}