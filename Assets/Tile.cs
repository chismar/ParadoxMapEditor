using UnityEngine;
using System.Collections;

public class Tile
{
    public int X { get; set; }
    public int Y { get; set; }
    Province owner;
    public Province Province { get { return owner; } set
        {
            if (owner == value)
                return;
            if (owner != null)
                owner.DetachTile(this);
            owner = value;
            if (!owner.HasTile(this))
                owner.AttachTile(this);
        }
    }
    public int BorderCount = 0;
}
