﻿using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
    public class AutofacEventTests
    {
        class Event : IEvent
        {
            public int Tally { get; set; }
        }

        class EventHandler : IEventHandler<Event>
        {
            public Task Handle(Event @event)
            {
                @event.Tally += 1;

                return Task.CompletedTask;
            }
        }

        class EventHandler2 : IEventHandler<Event>
        {
            public Task Handle(Event @event)
            {
                @event.Tally += 1;

                return Task.FromResult(0);
            }
        }

        [Fact]
        public async Task TestEvent()
        {
            var busBuilder = new BusBuilder()
                .RegisterEventHandler<Event, EventHandler>();

            var container = new ContainerBuilder().RegisterMicroBus(busBuilder).Build();

            var bus = container.Resolve<IMicroBus>();

            var @event = new Event();
            await bus.PublishAsync(@event);
            
            @event.Tally.Should().Be(1);
        }

        [Fact]
        public async Task TestMultipleEvents()
        {
            var busBuilder = new BusBuilder()
                .RegisterEventHandler<Event, EventHandler>()
                .RegisterEventHandler<Event, EventHandler2>();

            var container = new ContainerBuilder().RegisterMicroBus(busBuilder).Build();

            var bus = container.Resolve<IMicroBus>();

            var @event = new Event();
            await bus.PublishAsync(@event);

            @event.Tally.Should().Be(2);
        }
    }
}
