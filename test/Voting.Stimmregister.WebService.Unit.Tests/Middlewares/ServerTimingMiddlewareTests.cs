// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Voting.Stimmregister.WebService.Middlewares;
using Xunit;

namespace Voting.Stimmregister.WebService.Unit.Tests.Middlewares;

public class ServerTimingMiddlewareTests
{
    [Fact]
    public void BuildHeaderString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var activities = NewActivities();

        var activity1 = new Activity("FetchData").Start();
        activity1.SetEndTime(DateTime.UtcNow.AddMilliseconds(100));
        activity1.Stop();

        var activity2 = new Activity("RenderPage").Start();
        activity2.SetEndTime(DateTime.UtcNow.AddMilliseconds(200));
        activity2.Stop();

        var activity3 = new Activity("RenderPage").Start();
        activity3.SetEndTime(DateTime.UtcNow.AddMilliseconds(200));
        activity3.Stop();

        InvokePrivateMethod(activities, "Add", activity1);
        InvokePrivateMethod(activities, "Add", activity2);
        InvokePrivateMethod(activities, "Add", activity3);

        // Act
        var headerString = (string?)InvokePrivateMethod(activities, "BuildHeaderString");

        // Assert
        Assert.Contains("FetchData;dur=", headerString);
        Assert.Contains("RenderPage;dur=", headerString);
        Assert.DoesNotContain(",FetchData", headerString);
        Assert.Equal("FetchData;dur=100;desc=\"FetchData ()\",RenderPage;dur=400;desc=\"RenderPage ()\"", headerString);
    }

    [Fact]
    public void BuildHeaderString_ShouldReturnEmptyString_WhenNoActivities()
    {
        // Arrange
        var activities = NewActivities();

        // Act
        var headerString = (string?)InvokePrivateMethod(activities, "BuildHeaderString");

        // Assert
        Assert.Equal(string.Empty, headerString);
    }

    [Fact]
    public async Task InvokeAsync_ShouldAppendServerTimingHeader_UsingTestServer()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services => services.AddSingleton<ServerTimingMiddleware>())
            .Configure(app =>
            {
                app.UseMiddleware<ServerTimingMiddleware>();
                app.Run(async _ =>
                {
                    using var activity = new Activity("TestOperation");
                    activity.Start();
                    await Task.Delay(50);
                    activity.Stop();
                });
            });

        using var server = new TestServer(builder);
        using var client = server.CreateClient();

        var response = await client.GetAsync("/");
        response.Headers.TryGetValues("Server-Timing", out var values);

        Assert.NotNull(values);
        Assert.Contains("TestOperation;dur=", values.First());
    }

    private static object? InvokePrivateMethod(object instance, string methodName, params object[] parameters)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        return method?.Invoke(instance, parameters);
    }

    private static object NewActivities()
    {
        var nestedType = typeof(ServerTimingMiddleware).GetNestedType("ServerTimingActivities", BindingFlags.NonPublic);
        var instance = nestedType != null ? Activator.CreateInstance(nestedType) : throw new NotSupportedException();
        return instance ?? throw new NotSupportedException();
    }
}
