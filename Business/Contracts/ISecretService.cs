
namespace DetectiveClub.Business.Contracts;

public interface ISecretService
{
    public StatusResult<ServiceStatus, int> AddSecret(SecretDto newSecret);
    public ServiceStatus RemoveSecret(int secretId);
    
    public ServiceStatus EditSecret(SecretDto toEditSecret);
    
    public StatusResult<ServiceStatus, SecretDto> GetSecret(int secretId);

    public StatusResult<ServiceStatus, IEnumerable<SecretDto>> GetSecretsByEnvironment(int environmentId);

    public StatusResult<ServiceStatus, IEnumerable<SecretDto>> GetSecretsByType(int typeId);

    public StatusResult<ServiceStatus, int> AddSecretType(SecretTypeDto newSecretType);

    public ServiceStatus RemoveSecretType(int secretTypeId);

    public ServiceStatus EditSecretType(SecretTypeDto toEditSecretType);

    public StatusResult<ServiceStatus, SecretTypeDto> GetSecretType(int secretId);

    public IEnumerable<SecretTypeDto> GetSecretTypes();

}