using Blazocious.Core.Builder.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using System.Timers;

namespace Blazocious.Core.Builder;

public partial class ElementBuilder
{
    private static readonly ConcurrentDictionary<string, System.Timers.Timer> _debounceTimers = new();
    private readonly Dictionary<string, object> _styles = new();
    private Action? _onUpdate;
    private object? _previousState;

    public ElementBuilder OnChangeDebounced(int milliseconds, EventCallback<ChangeEventArgs> callback)
    {
        var timerId = Guid.NewGuid().ToString();

        return Attr("onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, async (args) =>
        {
            if (_debounceTimers.TryGetValue(timerId, out var existingTimer))
            {
                existingTimer.Stop();
                existingTimer.Dispose();
            }

            var timer = new System.Timers.Timer(milliseconds);
            timer.AutoReset = false;
            timer.Elapsed += async (s, e) =>
            {
                await callback.InvokeAsync(args);
                timer.Dispose();
                _debounceTimers.TryRemove(timerId, out _);
            };

            _debounceTimers.TryAdd(timerId, timer);
            timer.Start();
        }));
    }

    public ElementBuilder OnInputThrottled(int milliseconds, EventCallback<ChangeEventArgs> callback)
    {
        DateTime lastRun = DateTime.MinValue;

        return Attr("oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, async (args) =>
        {
            var now = DateTime.UtcNow;
            if ((now - lastRun).TotalMilliseconds >= milliseconds)
            {
                lastRun = now;
                await callback.InvokeAsync(args);
            }
        }));
    }

    public ElementBuilder WithState<T>(ElementState<T> state)
    {
        _previousState = state.Value;
        state = new ElementState<T>(
            state.Value,
            () =>
            {
                _onUpdate?.Invoke();
                _previousState = state.Value;
            }
        );
        return this;
    }

    public ElementBuilder OnUpdate(Action callback)
    {
        _onUpdate = callback;
        return this;
    }

    public ElementBuilder StyleObject(object styles)
    {
        var type = styles.GetType();
        var properties = type.GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(styles)?.ToString();
            if (value != null)
            {
                var name = ToCssName(prop.Name);
                _styles[name] = value;
            }
        }

        return this;
    }

    internal ElementBuilder AddStyle(string name, string value)
    {
        _styles.Add(name, value);
        return this;
    }

    private static string ToCssName(string pascalCase)
    {
        return string.Concat(
            pascalCase.Select((c, i) => i > 0 && char.IsUpper(c) ? $"-{char.ToLower(c)}" : char.ToLower(c).ToString())
        );
    }
}