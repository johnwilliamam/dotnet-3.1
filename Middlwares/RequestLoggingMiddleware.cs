using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capturar os par�metros da query string
        var queryParams = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;

        // Permitir que o corpo da requisi��o seja lido v�rias vezes
        context.Request.EnableBuffering();

        string body;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // Resetar o stream para o pr�ximo middleware
        }

        // Obter a atividade atual e adicionar as informa��es
        var activity = Activity.Current;
        if (activity != null)
        {
            // Adicionar o payload como tag
            activity.SetTag("http.request.body", body);

            // Adicionar os query params como tag
            activity.SetTag("http.request.query_params", queryParams);
        }

        await _next(context);
    }
}



