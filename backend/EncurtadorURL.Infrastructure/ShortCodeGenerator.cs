using EncurtadorURL.Application.Interfaces;
using HashidsNet;

namespace EncurtadorURL.Infrastructure.Services
{
    public class ShortCodeGenerator : IShortCodeGenerator
    {
        private readonly Hashids _hashids;
        private readonly object _syncLock = new object();
        private const string Base62Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public ShortCodeGenerator()
        {
            // O Hashids fará a ofuscação
            _hashids = new Hashids("NossaChaveSecretaParaODesafio", 6, Base62Alphabet);
        }
        public string Generate(int uniqueId)
        {
            // Requisito: processar apenas uma requisição por vez de forma sincronizada
            lock (_syncLock)
            {
                // O Hashids já aplica a lógica do Base62 internamente devido ao alfabeto configurado,
                // garantindo a ofuscação de um ID sequencial.
                return _hashids.Encode(uniqueId);
            }
        }
    }
}
