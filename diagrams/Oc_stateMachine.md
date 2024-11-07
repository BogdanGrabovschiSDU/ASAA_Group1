stateDiagram-v2
        [*] --> Listening
        Listening --> [*]: Panic
        	state msgType <<fork>>
        Listening --> msgType:Recieve Message
		state msgJoin <<join>>
		msgType --> Order
		msgType --> Ack
		Ack --> Log
		Log --> msgJoin
		msgType --> Fault
		Fault --> Recover
		Order --> Parse
		Parse --> Bus

		Recover --> Bus: Recover Msg
		Bus --> msgJoin
		msgJoin --> Listening
