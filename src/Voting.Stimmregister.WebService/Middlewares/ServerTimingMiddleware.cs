// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Voting.Stimmregister.WebService.Middlewares;

public class ServerTimingMiddleware : IMiddleware
{
    private const string ServerTimingHeaderName = "Server-Timing";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var activities = new ServerTimingActivities();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = _ => true;
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.PropagationData;
        listener.ActivityStopped = a => activities.Add(a);
        ActivitySource.AddActivityListener(listener);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append(
                ServerTimingHeaderName,
                activities.BuildHeaderString());
            return Task.CompletedTask;
        });

        await next(context);
    }

    private sealed record ServerTimingActivity(string Name, TimeSpan Duration, string? Description);

    private sealed class ServerTimingActivities
    {
        private readonly ConcurrentDictionary<string, ServerTimingActivity> _activities = new();

        public void Add(Activity activity)
        {
            _activities.AddOrUpdate(
                activity.OperationName,
                static (name, newActivity) => new ServerTimingActivity(name, newActivity.Duration, $"{newActivity.DisplayName} ({newActivity.Source.Name})"),
                static (_, existingActivity, newActivity) => existingActivity with { Duration = existingActivity.Duration + newActivity.Duration },
                activity);
        }

        public string BuildHeaderString()
        {
            return string.Join(",", _activities.Values.Select(activity =>
                $"{activity.Name};dur={(int)activity.Duration.TotalMilliseconds};desc=\"{activity.Description}\""));
        }
    }
}
