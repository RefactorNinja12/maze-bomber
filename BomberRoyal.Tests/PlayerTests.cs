using BomberRoyal.Core.Enums;
using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BombFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using NSubstitute;


namespace BomberRoyal.Tests
{
    public class PlayerTests
    {
       
        [Theory]
        [InlineData(50, 250)]   
        [InlineData(100, 200)]  
        [InlineData(299, 1)]   
        public void TakeDamage_ShouldReduceHealth_AndSetIsHurt(int damage, int expectedHealth)
        {
            // Arrange
            var mockBus = Substitute.For<IEventBus>();
            var player = new Player(Vector2.Zero, mockBus);

            // Act
            player.TakeDamage(damage);

            // Assert
            Assert.Equal(expectedHealth, player.Health);
            Assert.True(player.IsHurt);
            Assert.False(player.IsDying);
        }

       
       
        [Theory]
        [InlineData(400)]
        [InlineData(350)]
        [InlineData(301)]
        public void TakeDamage_ToZeroHealth_ShouldStartDeath(int damage)
        {
            // Arrange
            var mockBus = Substitute.For<IEventBus>();
            var player = new Player(Vector2.Zero, mockBus);

            // Act
            player.TakeDamage(damage);

            // Assert
            Assert.True(player.IsDying);
            Assert.True(player.Health <= 0);
        }

        [Theory]
        [InlineData(1.3f)]
        [InlineData(2.0f)]
        [InlineData(5.0f)] 
        public void Update_ShouldPublish_PlayerDiedEvent_WhenDeathTimerExpires(float deltaTime)
        {
            // Arrange
            var mockBus = Substitute.For<IEventBus>();
            var player = new Player(Vector2.Zero, mockBus);

            player.TakeDamage(400);

            // Act
            player.Update(deltaTime);

            // Assert
            mockBus.Received().Publish(Arg.Any<PlayerDiedEvent>());
            Assert.False(player.IsAlive);
            Assert.False(player.IsDying);
        }

   
        [Theory]
        [InlineData(0, 0)]
        [InlineData(5, 10)]
        [InlineData(100, 50)]
        public void PlaceBomb_ShouldCall_BombSystem(float x, float y)
        {
            // Arrange
            var mockBus = Substitute.For<IEventBus>();
            var mockBombSystem = Substitute.For<IBombSystem>();
            var player = new Player(new Vector2(x, y), mockBus);
            player.AssignBombSystem(mockBombSystem);

            // Act
            player.PlaceBomb();

            // Assert
            mockBombSystem.Received().TryPlaceBomb(new Vector2(x, y));
        }

    
        [Theory]
        [InlineData(1, 0, Direction.Right)]   
        [InlineData(-1, 0, Direction.Left)]   
        [InlineData(0, 1, Direction.Down)]    
        [InlineData(0, -1, Direction.Up)]     
        public void Move_ShouldChangePosition_AndDirection(float dx, float dy, Direction expectedDirection)
        {
            
            var mockBus = Substitute.For<IEventBus>();
            var player = new Player(Vector2.Zero, mockBus);
            var dir = new Vector2(dx, dy);

            
            player.Move(dir, 1f);

            
            Assert.NotEqual(Vector2.Zero, player.Position);
            Assert.Equal(expectedDirection, player.Direction);
        }

     
        [Theory]
        [InlineData(0.3f, true)]   
        [InlineData(0.7f, false)] 
        [InlineData(1.0f, false)]  
        public void Update_ShouldReset_IsHurt_AfterTimerExpires(float deltaTime, bool expectedHurt)
        {
            
            var mockBus = Substitute.For<IEventBus>();
            var player = new Player(Vector2.Zero, mockBus);
            player.TakeDamage(50); 

            
            player.Update(deltaTime);

            
            Assert.Equal(expectedHurt, player.IsHurt);
        }
    }
}