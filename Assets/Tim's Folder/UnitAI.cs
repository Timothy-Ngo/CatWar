using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public List<Command> commands;

    
    // Start is called before the first frame update
    void Awake()
    {
        commands = new List<Command>();
    }

    // Update is called once per frame
    void Update()
    {
        if (commands.Count > 0) {
            if (commands[0].IsDone()) {
                StopAndRemoveCommand(0);
            } else {
                commands[0].Tick();
                commands[0].isRunning = true;
            }
        }
    }
    
    void StopAndRemoveCommand(int index)
    {
        commands[index].Stop();
        commands.RemoveAt(index);
    }
    public void StopAndRemoveAllCommands()
    {
        for(int i = commands.Count - 1; i >= 0; i--) {
            StopAndRemoveCommand(i);
        }
    }
    public void AddCommand(Command c)
    {
        //c.Init(); // Not needed since Init() usually draws lines which we don't want
        commands.Add(c);
    }
    
    public void SetCommand(Command c)
    {
        //print("Setting command: " + c.ToString());
        StopAndRemoveAllCommands();
        commands.Clear();
        AddCommand(c);

    }
}
