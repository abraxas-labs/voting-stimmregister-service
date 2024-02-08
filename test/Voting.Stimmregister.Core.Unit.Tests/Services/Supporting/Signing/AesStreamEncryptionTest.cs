// (c) Copyright by Abraxas Informatik AG
// For license information see LICENSE file

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Voting.Stimmregister.Abstractions.Adapter.Hsm;
using Voting.Stimmregister.Core.Services.Supporting.Signing;
using Voting.Stimmregister.Domain.Configuration;
using Voting.Stimmregister.Domain.Models.Signing;
using Xunit;

namespace Voting.Stimmregister.Core.Unit.Tests.Services.Supporting.Signing;

public class AesStreamEncryptionTest
{
    private readonly AesStreamEncryption _testee;
    private readonly Mock<IHsmCryptoAdapter> _hsmCryptoAdapterMock = new();

    public AesStreamEncryptionTest()
    {
        _hsmCryptoAdapterMock.Setup(m => m.GetSignatureConfigLabels()).Returns(new SignatureConfigLabels(string.Empty, string.Empty, string.Empty));
        _testee = new AesStreamEncryption(_hsmCryptoAdapterMock.Object);
    }

    [Theory]
    [InlineData("Neque porro quisquam est qui dolorem ipsum.")]
    [InlineData("Neque.")]
    public async Task AesEncryptStream_AesDecryptStream_WhenEncryptingAndDecrypting_ThenTheStreamShouldBeIdentical(string plainText)
    {
        var stringToEncryptBytes = Encoding.UTF8.GetBytes(plainText);

        _hsmCryptoAdapterMock
            .Setup(m => m.EncryptAes(It.IsAny<byte[]>(), It.IsAny<SymmetricKeyConfig>()))
            .Returns<byte[], SymmetricKeyConfig>((input, _) => input);

        _hsmCryptoAdapterMock
            .Setup(m => m.DecryptAes(It.IsAny<byte[]>(), It.IsAny<SymmetricKeyConfig>()))
            .Returns<byte[], SymmetricKeyConfig>((input, _) => input);

        await using var targetStream = new MemoryStream();
        var (cryptoStream, aesCipherMetadata) = _testee.CreateAesMacEncryptCryptoStream(targetStream);
        await cryptoStream.WriteAsync(stringToEncryptBytes);
        await cryptoStream.DisposeAsync();

        // make sure the hmac is set
        aesCipherMetadata.Hmac.Should().HaveCount(32);
        aesCipherMetadata.Hmac.Any(x => x != 0).Should().BeTrue();

        var inputStream = new MemoryStream(targetStream.ToArray());

        var decryptionStream = _testee.CreateAesMacDecryptCryptoStream(inputStream, aesCipherMetadata);
        var plainTextOutputStream = new MemoryStream();
        await decryptionStream.CopyToAsync(plainTextOutputStream);

        var plainTextAsBytes = plainTextOutputStream.ToArray();
        var plainTextAsString = Encoding.UTF8.GetString(plainTextAsBytes);

        plainTextAsString.Should().Be(plainText);
    }
}
