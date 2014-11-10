using UnityEngine;
using System.Collections;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Text;
using com.shephertz.app42.gaming.multiplayer.client.command;
using System;
using System.Threading;
using System.Collections.Generic;

public class ClientSample : MonoBehaviour
{
	private static int LastDiceNumber=0;
	public static string roomId = null;
	private static string localUser = "rahul";
	private static string opponentUser="";
	public static string LastTurn = "";
	private static string apiKey = "";
	private static string secretKey = "";
	ConnectionListener connListener;
	RoomListener roomListener;
	NotifyList notify;
	ZoneListen zoneListen;
	TurnListener turnListner;
	string debug = "";
	public void Start ()
	{
		initAppwarp ();
		connListener = new ConnectionListener ();
		roomListener = new RoomListener ();
		notify = new NotifyList ();
		zoneListen = new ZoneListen ();
		turnListner = new TurnListener ();
		WarpClient.GetInstance ().AddConnectionRequestListener (connListener);
		WarpClient.GetInstance ().AddRoomRequestListener (roomListener);
		WarpClient.GetInstance ().AddNotificationListener (notify);
		WarpClient.GetInstance ().AddZoneRequestListener (zoneListen);
		WarpClient.GetInstance ().AddTurnBasedRoomRequestListener (turnListner);
	}
	
	private void initAppwarp ()
	{
		WarpClient.initialize (apiKey, secretKey);
		WarpClient.setRecoveryAllowance (20);
	}
	
	public void Update ()
	{
		WarpClient.GetInstance ().Update ();
	}
	
	void OnGUI ()
	{
		GUI.contentColor = Color.white;
		GUI.Label (new Rect (10, 10, 300, 100)," Last Dice Number "+LastDiceNumber+ getDebug()+ " " +connListener.getDebug () + " " + zoneListen.getDebug () + " " + roomListener.getDebug () +" "+turnListner.getDebug() +" " + notify.getDebug ());
		
		if (GUI.Button (new Rect (10, 130, 150, 60), "Connect")) {		
			WarpClient.GetInstance ().Connect (localUser);
		}
		
		if (GUI.Button (new Rect (10, 200, 150, 60), "Create Room")) {		

			WarpClient.GetInstance ().CreateTurnRoom ("UnityRoom", "UnityRoom", 2, null,10);
		}

		if (GUI.Button (new Rect (180, 200, 150, 60), "Throw Dice")) {
			LastDiceNumber=UnityEngine.Random.Range(1,7);
		}

		if (GUI.Button (new Rect (350, 200, 150, 60), "Start Game")) {		
			WarpClient.GetInstance().startGame(false,localUser);
		}
		
		if (GUI.Button (new Rect (520, 200, 150, 60), "Stop Game")) {
			WarpClient.GetInstance ().stopGame();
		}

		if (GUI.Button (new Rect (10, 270, 150, 60), "Send Move")) {		
			if(LastDiceNumber==6)
				WarpClient.GetInstance().sendMove(LastDiceNumber.ToString(),localUser);
			else
				WarpClient.GetInstance().sendMove(LastDiceNumber.ToString(),opponentUser);
		}

		if (GUI.Button (new Rect (180, 270, 150, 60), "Set Next Turn")) {
			String nextTurn="";
			if(LastTurn.Equals(localUser))
			{
				if(LastDiceNumber!=6)
					nextTurn=opponentUser;
				else
					nextTurn=localUser;
			
			}else{

				if(notify.getLastDiceNumber()!=6)
					nextTurn=localUser;
				else
					nextTurn=opponentUser;
			}
			WarpClient.GetInstance().SetNextTurn(nextTurn);
		}
	}
	
	void OnApplicationQuit ()
	{

	}
	private void Log (string msg)
	{
		debug = msg + "  " + debug;
	}
	
	public string getDebug ()
	{
		return debug;
	}
	
}

public class ConnectionListener : ConnectionRequestListener
{
	string debug = "";
	private void Log (string msg)
	{
		debug = msg + "  " + debug;
	}
	
	public string getDebug ()
	{
		return debug;
	}
	
	public void onConnectDone (ConnectEvent e)
	{
		switch (e.getResult ()) {
		case WarpResponseResultCode.AUTH_ERROR:
			if (e.getReasonCode () == WarpReasonCode.WAITING_FOR_PAUSED_USER) {
				Log ("Auth Error:Server is waiting for previous session");
			} else {
				Log ("Auth Error:SessionId Expired");
			}
			break;
		case WarpResponseResultCode.SUCCESS:
			Log ("Connect Done ");
			break;
		case WarpResponseResultCode.CONNECTION_ERROR_RECOVERABLE:
			Log ("Connect Error Recoverable ");
			break;
		case WarpResponseResultCode.SUCCESS_RECOVERED:
			Log ("Connection Success Recovered");
			break;
		default:
			Log ("Connect Failed " + e.getResult ());
			break;
		}
	}
	
	public void onLog (string logs)
	{
		Log (logs);
	}
	
	public void onDisconnectDone (ConnectEvent e)
	{
		if (e.getResult () == 0) {
			Log ("Disconnect Done ");
		} else {
			Log ("Disconnect Failed " + e.getResult ());
		}
	}
	
	public void onInitUDPDone (byte bytes)
	{
	}
}
public class TurnListener:TurnBasedRoomListener
{
	#region TurnBasedRoomListener implementation
	string debug = "";
	
	private void Log (string msg)
	{
		debug = msg + "  " + debug;
	}
	
	public string getDebug ()
	{
		return debug;
	}

	public void onSendMoveDone (byte result)
	{
		Log ("onSendMoveDone "+result);
	}

	public void onStartGameDone (byte result)
	{
		Log ("onStartGameDone "+result);
	}

	public void onStopGameDone (byte result)
	{
		Log ("onStopGameDone "+result);
	}

	public void onSetNextTurnDone (byte result)
	{
		Log ("onSetNextTurnDone "+result);
	}

	public void onGetMoveHistoryDone (byte result, MoveEvent[] moves)
	{
		throw new NotImplementedException ();
	}

	#endregion
}
public class RoomListener: RoomRequestListener
{
	
	string debug = "";
	
	private void Log (string msg)
	{
		debug = msg + "  " + debug;
	}
	
	public string getDebug ()
	{
		return debug;
	}
	
	public void onSubscribeRoomDone (RoomEvent eventObj)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onUnSubscribeRoomDone (RoomEvent eventObj)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onLeaveRoomDone (RoomEvent eventObj)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onGetLiveRoomInfoDone (LiveRoomInfoEvent eventObj)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onSetCustomRoomDataDone (LiveRoomInfoEvent eventObj)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onUpdatePropertyDone (LiveRoomInfoEvent liveRoomInfoEvent)
	{
		Log ("Result :" + liveRoomInfoEvent.getResult () + "Count :" + liveRoomInfoEvent.getProperties ().Count);
	}
	
	public void onLockPropertiesDone (byte result)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onUnlockPropertiesDone (byte result)
	{
		//throw new System.NotImplementedException ();
	}
	/// Invoked when the response for joinRoom request is received.
	/// <param name="eventObj"></param>
	public void onJoinRoomDone (RoomEvent eventObj)
	{
		Log ("on JoinRoomDone " + eventObj.getResult ());
	}
	/// other methods
}

public class NotifyList:NotifyListener
{
	#region NotifyListener implementation
	string debug = "";
	
	private void Log (string msg)
	{
		debug = msg + "  " + debug;
	}
	
	public string getDebug ()
	{
		return debug;
	}
	private int LastDiceNumber = 0;

	public int getLastDiceNumber()
	{
		return LastDiceNumber;
	}

	public void onRoomCreated (RoomData eventObj)
	{

	}
	
	public void onRoomDestroyed (RoomData eventObj)
	{

	}
	
	public void onUserLeftRoom (RoomData eventObj, string username)
	{
		Log ("onUserLeftRoom " + username);
	}
	
	public void onUserJoinedRoom (RoomData eventObj, string username)
	{
		Log ("onUserJoinedRoom " + username);
	}
	
	public void onUserLeftLobby (LobbyData eventObj, string username)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onUserJoinedLobby (LobbyData eventObj, string username)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onChatReceived (ChatEvent eventObj)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onUpdatePeersReceived (UpdateEvent eventObj)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onUserChangeRoomProperty (RoomData roomData, string sender, System.Collections.Generic.Dictionary<string, object> properties, System.Collections.Generic.Dictionary<string, string> lockedPropertiesTable)
	{
		//throw new System.NotImplementedException ();
	}
	
	public void onPrivateChatReceived (string sender, string message)
	{
		//hrow new System.NotImplementedException ();
	}
	
	public void onMoveCompleted (MoveEvent moveEvent)
	{
		Log ("onMoveCompleted " + moveEvent.getMoveData().ToString()+" next turn "+moveEvent.getNextTurn());
	}
	
	public void onUserPaused (string locid, bool isLobby, string username)
	{
		Log ("onUserPaused " + username);
	}
	
	public void onUserResumed (string locid, bool isLobby, string username)
	{
		Log ("onUserResumed " + username);
	}
	
	public void onGameStarted (string sender, string roomId, string nextTurn)
	{
		Log ("onGameStarted " + sender);
	}
	
	public void onGameStopped (string sender, string roomId)
	{
		Log ("onGameStopped " + sender);
	}
	
	public void onPrivateUpdateReceived (string sender, byte[] update, bool fromUdp)
	{
		Log (sender + " " + Encoding.UTF8.GetString (update, 0, update.Length) + " " + fromUdp);
	}

	public void onNextTurnRequest (string lastTurn)
	{
		Log ("onNextTurnRequest " + lastTurn);
	}

	#endregion
}

public class ZoneListen:ZoneRequestListener
{
	string debug = "";
	
	private void Log (string msg)
	{
		debug = msg + "  " + debug;
	}
	
	public string getDebug ()
	{
		return debug;
	}
	
	public void onDeleteRoomDone (RoomEvent eventObj)
	{
		throw new System.NotImplementedException ();
	}
	
	public void onGetAllRoomsDone (AllRoomsEvent eventObj)
	{
		throw new System.NotImplementedException ();
	}
	
	public void onCreateRoomDone (RoomEvent eventObj)
	{
		ClientSample.roomId = eventObj.getData ().getId ();
		WarpClient.GetInstance ().JoinRoom (ClientSample.roomId);
	}
	
	public void onGetOnlineUsersDone (AllUsersEvent eventObj)
	{
		throw new System.NotImplementedException ();
	}
	
	public void onGetLiveUserInfoDone (LiveUserInfoEvent eventObj)
	{
		throw new System.NotImplementedException ();
	}
	
	public void onSetCustomUserDataDone (LiveUserInfoEvent eventObj)
	{
		throw new System.NotImplementedException ();
	}
	
	public void onGetMatchedRoomsDone (MatchedRoomsEvent matchedRoomsEvent)
	{
		throw new System.NotImplementedException ();
	}
}
