using System;
using Xunit;

namespace Wallpaper.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Set()
        {
            int result = Wallpaper.SetDesktopWallpaper("a");
            Assert.Equal(1, result);
        }

        [Fact]
        public void Get()
        {
            string wallpaper = Wallpaper.GetDesktopWallpaper();
            Assert.True(wallpaper.Length > 0);
        }
    }
}
