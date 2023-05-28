Names:          UPIS:
Yiyang Chen     ych363
Dave Walton     dwal914

Changes on FiniteStateMachine.cs: 
1. Add a method [public string GetNextState()] 
2. Add a method [public void ExtendTable(string state, string eventTrigger)] 
3. Add a method [private StateInformation GetDefaultElement()]


Changes on Task2.cs
Main change: Virtual keyword added in function statement so we can override them in Task 3
1. Changed constructor content
2. Add a method [public void WriteInFile(string message)]
3. change [void Start();] -> [async Task Start();]

In the IController.cs, [void  Start();] is changed to [Task  Start();]
In the TaskPage.xaml.cs, [_controller.Start();] is changed to [await  _controller.Start();]

4. Add a method [public TrafficLightState MatchEnumFinite(string state)]
5. Add a action [public async void SendToMC(DateTime timestamp)]
6. Add a action [public void WriteToLog(DateTime timestamp)]
7. Add a action [public void UpdateGUI(DateTime timestamp)]


Changes on Task3.cs
1. Override method [public override void ConfigLightLength(int redLength, int greenLength)]
2. Override method [public override void ExitConfigMode()]
3. Override method [public override async Task<bool> EnterConfigMode()]
4. Override method [public override async Task Start()]
5. Add a method [private void OnTimeEvent(Object sender, ElapsedEventArgs e)]
6. Add a method [public void SetDelay()]
7. Override method [public override void Tick()]