using System.Collections;

public abstract class FSM {

    public float TicksPerSecond { get; protected set; }

    protected bool isRunning = true;

    public FSM() {
        TicksPerSecond = 1f;
    }

    public abstract IEnumerator run();

    public void Stop() {
        this.isRunning = false;
    }

}
