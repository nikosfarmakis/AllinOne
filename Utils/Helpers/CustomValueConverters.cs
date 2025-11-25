using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AllinOne.Utils.Helpers
{
    public class CustomValueConverters
    {
        public static readonly ValueConverter<string, string> EncryptStringConverter =
            new ValueConverter<string, string>(
                v => v == null ? null : AesGcmEncryptionHelper.Encrypt(v),
                v => v == null ? null : AesGcmEncryptionHelper.Decrypt(v)
            );

        public static readonly ValueConverter<int, string> EncryptIntConverter =
            new ValueConverter<int, string>(
                v => AesGcmEncryptionHelper.Encrypt(v.ToString()),
                v => int.Parse(AesGcmEncryptionHelper.Decrypt(v))
            );

        public static readonly ValueConverter<bool, string> EncryptBoolConverter =
            new ValueConverter<bool, string>(
                v => AesGcmEncryptionHelper.Encrypt(v.ToString()),
                v => bool.Parse(AesGcmEncryptionHelper.Decrypt(v))
            );

        public static readonly ValueConverter<DateTime?, string> EncryptDateTimeConverter =
            new ValueConverter<DateTime?, string>(
                v => v.HasValue ? AesGcmEncryptionHelper.Encrypt(v.Value.ToString("O")) : null,
                v => string.IsNullOrEmpty(v) ? (DateTime?)null :
                     DateTime.Parse(AesGcmEncryptionHelper.Decrypt(v), null, System.Globalization.DateTimeStyles.RoundtripKind)
            );
    }
}
