##################################################
#			AUDECTRA CLIENT EXTENSION
#
#	FILENAME:	UDPEthernet.py
#	VERSION:	v1.0.0
#
#	DESCRIPTION:
#		Sends R,G,B Values with UDP over Ethernet.
#
#	NOTES:
#		Native Extension, included with Audectra.
#
#	AUTHOR:		Audectra e.U.
#

# Load Assemblies
clr.AddReference("System.Net")

# Import from Assemblies
from System import Array, Byte
from System.Net import IPEndPoint, IPAddress
from System.Net.Sockets import UdpClient

# Name shown in the add client dropdown menu
ClientType = "UDP"

# Agent class, responsible for the creation of a new client.
class UDPClientAgent (cClientAgentBase):
	def __init__(self):
		self.ClientType = ClientType

	# Generate the elements for the add client dialog.
	def GenerateElements(self):
		ParameterList = List[AddClientParameter]()
		
		# Textbox for the IP Address
		box = TextBox()
		title = "IP Address:"
		ParameterList.Add(AddClientParameter(title, box))
		
		# Textbox for the Port
		box = TextBox()
		title = "Port:"
		ParameterList.Add(AddClientParameter(title, box))
		
		return ParameterList

	# Create a new client with provided information.
	def NewClient(self, ParameterList):
		# Get the specified IP Address
		box = ParameterList[0].textBox
		ip = box.Text
		
		# Get the specified Port
		box = ParameterList[1].textBox
		port = box.Text

		# Check if items where filled
		if (ip is "" or port is ""):
			return None
		
		# Create and return the client
		client = UDPClient(ip, port)
		return client

	
# Actual client class, responsible for the communication.
class UDPClient (cClientBase):
	def __init__(self, ip, port):
		self.ClientType = ClientType
		self.ID = "UDP Client"
		self.ConnectionAddress = ip + ":" + port
		
		# Connect upon creation
		self._Endpoint = IPEndPoint(IPAddress.Parse(ip), int(port))
		self._Connected = False
		self.Connect()
		
	# Return the connection state of the client.
	def IsConnected(self):
		return self._Connected
		
	# Connect to client, return True if successfull.
	def Connect(self):
		if not self._Connected:
			try:
				self._UDPClient = UdpClient(0)
				self._UDPClient.Connect(self._Endpoint)
				self._Connected = True
			except:
				self._Connected = False
			
		return self._Connected
	
	# Disconnect from client.
	def Disconnect(self):
		if self._Connected:
			self._UDPClient.Close()
			self._Connected = False
	
	# Send the provided R,G,B values to client.
	def Send(self, R, G, B):
		if self._Connected:
			barray = Array.CreateInstance(Byte, 3)
			
			barray[0] = R * 255
			barray[1] = G * 255
			barray[2] = B * 255
			
			self._UDPClient.Send(barray, 3)
			return True
		else:
			return False

