using System.Collections.Concurrent;
using EncurtadorURL.Infrastructure.Services;
using Xunit;

namespace EncurtadorURL.Tests
{
    public class ShortCodeGeneratorTests
    {
        [Fact]
        public async Task TeGenerate_DeveRetornarCodigosUnicos_QuandoExecutadoConcorrentemente()
        {
            // Arrange
            var generator = new ShortCodeGenerator();
            var codes = new ConcurrentBag<string>();
            int totalRequests = 500;
            var tasks = new List<Task>();

            // Act - Simula m�ltiplas requisi��es simult�neas para testar o comportamento sincronizado
            for (int i = 1; i <= totalRequests; i++)
            {
                int id = i;
                tasks.Add(Task.Run(() =>
                {
                    var code = generator.Generate(id);
                    codes.Add(code);
                }));
            }

            await Task.WhenAll(tasks.ToArray());
            
            // Assert
            Assert.Equal(totalRequests, codes.Count);
            Assert.Equal(totalRequests, codes.Distinct().Count()); // Garante que nenhum c�digo foi duplicado
        }
    }
    
}