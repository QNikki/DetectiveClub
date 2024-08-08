using DetectiveClub.Data;

namespace DetectiveClub.Business;

public interface ISecretService
{
    public StatusResult<ServiceStatus, int> AddSecret(Secret newSecret);
    public ServiceStatus RemoveSecret(int secretId);
    
    public ServiceStatus EditSecret(Secret toEditSecret);
    
    public Secret? GetSecret(int secretId);

    public IEnumerable<Secret> GetSecretsByEnvironment(int environmentId);

    public IEnumerable<Secret> GetSecretsByType(int typeId);

    public StatusResult<ServiceStatus, int> AddSecretType(SecretType newSecretType);

    public ServiceStatus RemoveSecretType(int secretTypeId);

    public ServiceStatus EditSecretType(SecretType toEditSecretType);

    public SecretType? GetSecretType(int secretId);

    public IEnumerable<SecretType> GetSecretTypes();

}