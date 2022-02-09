namespace AniGate.Core;

public abstract class GuidEntity
{
    private string _id = string.Empty;

    public string Id
    {
        get => _id;
        set
        {
            if (!Guid.TryParse(value, out var guid) || guid == Guid.Empty)
                throw new ArgumentException($"\"{value}\" is not a valid GUID.");
            _id = value;
        }
    }

    public void SetNewGuid() => _id = Guid.NewGuid().ToString();
}