namespace EStore.Application.Models.Configuration;

public class WebhooksConfiguration
{
    public N8nConfiguration N8n { get; set; }
}

public class N8nConfiguration
{
    public string ConfirmEmail { get; set; }
}
