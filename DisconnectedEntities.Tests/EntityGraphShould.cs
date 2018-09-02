using DisconnectedEntities.Tests.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace DisconnectedEntities.Tests
{
    [Collection("EfCollection")]
    public class EntityGraphShould
    {
        private readonly EfFixture _fixture;
        private readonly ITestOutputHelper _logger;

        public EntityGraphShould(EfFixture fixture, ITestOutputHelper logger)
        {
            _fixture = fixture;
            _logger = logger;
        }

        [Fact]
        public void set_to_added_state_all_entities_regardless_they_have_or_not_a_key_value_when_adding_a_graph()
        {
            var customer = new Customer
            {
                Country = new Country()
                {
                    Id = 1,
                    Name = "Spain"
                },
                Orders = new Collection<Order>()
                {
                    new Order() { Units = 1, Amount = 10m },
                    new Order() { Units = 2, Amount = 20m }
                }
            };

            using (var context = _fixture.CreateDbContext())
            {
                context.Customers.Add(customer);

                context.ChangeTracker.Entries().Should().HaveCount(4);
                context.ChangeTracker.Entries().Should().OnlyContain(entry => entry.State == EntityState.Added);
            }
        }

        [Fact]
        public void set_to_added_state_entities_without_a_key_value_and_to_modified_state_entities_with_a_key_value_when_updating_a_graph()
        {
            var customer = new Customer
            {
                Country = new Country()
                {
                    Name = "Spain"
                },
                Orders = new Collection<Order>()
                {
                    new Order() { Id = 1, Units = 1, Amount = 10m },
                    new Order() { Units = 2, Amount = 20m }
                }
            };

            using (var context = _fixture.CreateDbContext())
            {
                context.Customers.Update(customer);

                context.ChangeTracker.Entries().Should().HaveCount(4);

                context.Entry(customer).State.Should().Be(EntityState.Added);
                context.Entry(customer.Country).State.Should().Be(EntityState.Added);
                context.ChangeTracker.Entries<Order>().Should().ContainSingle(entry => entry.State == EntityState.Modified && entry.Entity.Id == 1);
                context.ChangeTracker.Entries<Order>().Should().ContainSingle(entry => entry.State == EntityState.Added);
            }
        }

        [Fact]
        public void set_to_deleted_state_root_entity_with_a_key_value_and_to_unchanged_state_child_entities_with_a_key_value_and_to_added_state_child_entities_without_a_key_value_when_removing_a_graph()
        {
            var customer = new Customer
            {
                Id = 1,
                Country = new Country()
                {
                    Id = 1,
                    Name = "Spain"
                },
                Orders = new Collection<Order>()
                {
                    new Order() { Units = 1, Amount = 10m },
                    new Order() { Units = 2, Amount = 20m }
                }
            };

            using (var context = _fixture.CreateDbContext())
            {
                context.Customers.Remove(customer);

                context.ChangeTracker.Entries().Should().HaveCount(4);
                context.Entry(customer).State.Should().Be(EntityState.Deleted);
                context.Entry(customer.Country).State.Should().Be(EntityState.Unchanged);
                context.ChangeTracker.Entries<Order>().Should().OnlyContain(entry => entry.State == EntityState.Added);
            }
        }

        [Fact]
        public void throws_an_exception_when_removing_a_graph_with_a_root_entity_with_auto_generated_key_without_a_key_value()
        {
            using (var context = _fixture.CreateDbContext())
            {
                Action action = () => context.Customers.Remove(new Customer());
                action.Should().Throw<InvalidOperationException>();
            }
        }

        [Fact]
        public void does_not_throw_an_exception_when_removing_a_graph_with_a_root_entity_with_no_auto_generated_key_without_a_key_value()
        {
            using (var context = _fixture.CreateDbContext())
            {
                context.Products.Remove(new Product());
            }
        }

        [Theory]
        [InlineData(0, EntityState.Added)]
        [InlineData(1, EntityState.Added)]
        [InlineData(0, EntityState.Modified)]
        [InlineData(1, EntityState.Modified)]
        [InlineData(0, EntityState.Deleted)]
        [InlineData(1, EntityState.Deleted)]
        public void attach_root_entity_and_set_to_desired_state_regardless_it_has_or_not_a_key_value_and_ignore_child_entities_when_use_entry_method(int customerId, EntityState state)
        {
            var customer = new Customer
            {
                Id = customerId,
                Country = new Country()
                {
                    Id = 1,
                    Name = "Spain"
                },
                Orders = new Collection<Order>()
                {
                    new Order() { Units = 1, Amount = 10m },
                    new Order() { Units = 2, Amount = 20m }
                }
            };

            using (var context = _fixture.CreateDbContext())
            {
                context.Entry(customer).State = state;

                context.ChangeTracker.Entries().Should().HaveCount(1);
                context.Entry(customer).State.Should().Be(state);
            }
        }

        [Theory]
        [InlineData(EntityState.Added)]
        [InlineData(EntityState.Modified)]
        [InlineData(EntityState.Deleted)]
        public void attach_graph_and_set_root_entity_to_desired_state_regardless_it_has_or_not_a_key_value_and_to_added_state_child_entities_without_key_value_and_to_unchanged_state_child_entities_with_a_key_value_when_attaching_a_graph_and_set_entity_state_to_added(EntityState state)
        {
            var customer = new Customer
            {
                Id = 1,
                Country = new Country()
                {
                    Id = 1,
                    Name = "Spain"
                },
                Orders = new Collection<Order>()
                {
                    new Order() { Id = 1, Units = 1, Amount = 10m },
                    new Order() { Units = 2, Amount = 20m }
                }
            };

            using (var context = _fixture.CreateDbContext())
            {
                context.Attach(customer).State = state;

                context.ChangeTracker.Entries().Should().HaveCount(4);
                context.Entry(customer).State.Should().Be(state);
                context.Entry(customer.Country).State.Should().Be(EntityState.Unchanged);
                context.ChangeTracker.Entries<Order>().Should().ContainSingle(entry => entry.State == EntityState.Added);
                context.ChangeTracker.Entries<Order>().Should().ContainSingle(entry => entry.State == EntityState.Unchanged);
            }
        }

        [Theory]
        [InlineData(EntityState.Modified)]
        [InlineData(EntityState.Deleted)]
        public void throws_an_exception_when_attaching_a_root_entity_with_auto_generated_key_without_a_key_value_and_desired_entity_state(EntityState state)
        {
            using (var context = _fixture.CreateDbContext())
            {
                Action action = () => context.Customers.Attach(new Customer()).State = state;
                action.Should().Throw<InvalidOperationException>();
            }
        }

        [Fact]
        public void choose_an_appropriate_entity_state_for_each_entity_in_the_graph_when_calling_track_graph_method()
        {
            var customer = new Customer
            {
                Id = 1,
                Country = new Country()
                {
                    Id = 1,
                    Name = "Spain"
                },
                Orders = new Collection<Order>()
                {
                    new Order() { Units = 1, Amount = 10m },
                    new Order() { Units = 2, Amount = 20m }
                }
            };

            using (var context = _fixture.CreateDbContext())
            {
                context.ChangeTracker.TrackGraph(customer, (EntityEntryGraphNode e) =>
                {
                    e.Entry.State = e.Entry.IsKeySet ? EntityState.Modified : EntityState.Added;
                });

                context.ChangeTracker.Entries().Should().HaveCount(4);
                context.Entry(customer).State.Should().Be(EntityState.Modified);
                context.Entry(customer.Country).State.Should().Be(EntityState.Modified);
                context.ChangeTracker.Entries<Order>().Should().OnlyContain(entry => entry.State == EntityState.Added);
            }
        }

        [Fact]
        public void not_track_and_entity_if_not_entity_state_is_set_when_calling_track_graph_method()
        {
            var customer = new Customer
            {
                Id = 1,
                Country = new Country()
                {
                    Id = 1,
                    Name = "Spain"
                },
                Orders = new Collection<Order>()
                {
                    new Order() { Units = 1, Amount = 10m },
                    new Order() { Units = 2, Amount = 20m }
                }
            };

            using (var context = _fixture.CreateDbContext())
            {
                context.ChangeTracker.TrackGraph(customer, (EntityEntryGraphNode e) =>
                {
                    if (e.Entry.Entity is Customer || e.Entry.Entity is Country)
                        e.Entry.State = e.Entry.IsKeySet ? EntityState.Modified : EntityState.Added;
                });

                context.ChangeTracker.Entries().Should().HaveCount(2);
                context.Entry(customer).State.Should().Be(EntityState.Modified);
                context.Entry(customer.Country).State.Should().Be(EntityState.Modified);
            }
        }

    }
}
