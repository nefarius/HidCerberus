namespace HidCerberus.Core.Firewall.ProcessIdentifiers
{
    public interface IProcessIdentifier
    {
        bool IdentifyByPid(int pid);
    }
}