[System.Serializable]
public class Quest
{
    public string ID { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsCompleted { get; private set; }

    public Quest(string id, string title, string description)
    {
        ID = id;
        Title = title;
        Description = description;
        IsCompleted = false;
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}
