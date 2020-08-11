using System.IO;

using Android.OS;

using Autofac;

using ScreenReaderService.Dto;
using ScreenReaderService.Services;

namespace ScreenReaderService.Data.Services
{
    public class SessionService : ObjectFileMappingService<Session>
    {
        private const string SESSION_INFO_FILE_NAME = "session.json";

        private static readonly string SESSION_INFO_PATH = Path.Combine(
            Environment.ExternalStorageDirectory.AbsolutePath, "CourierBot"
        );

        public SessionService() : base(SESSION_INFO_PATH, SESSION_INFO_FILE_NAME) { }

        public Session Session
        {
            get => Data;
            set => Data = value;
        }

        private SaltService SaltService = DependencyHolder.Dependencies.Resolve<SaltService>();

        public SessionDto GetRandomSessionDto()
        {
            SessionDto dto = new SessionDto();

            dto.UserId = Session.UserId;
            dto.Salt = SaltService.GetRandomSalt();
            dto.SessionTokenSalted = SaltService.GetSaltedHash(Session.Token, dto.Salt);

            return dto;
        }
    }
}