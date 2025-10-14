using NUnit.Framework;

public class ChoiceManagerTests
{
    [Test]
    public void SetFlag_And_GetFlag_Works()
    {
        ChoiceManager.Instance.SetFlag("door_open", true);
        Assert.IsTrue(ChoiceManager.Instance.GetFlag<bool>("door_open"));
    }

    [Test]
    public void Subscribe_Receives_Changes()
    {
        object seen = null;
        ChoiceManager.Instance.SubscribeToFlagChanges("key", v => seen = v);
        ChoiceManager.Instance.SetFlag("key", 5);
        Assert.AreEqual(5, seen);
    }
}


