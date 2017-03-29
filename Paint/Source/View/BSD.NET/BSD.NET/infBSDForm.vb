Friend Interface BSDForm
    ReadOnly Property FormName() As String
    Property PLCData() As String()
    Property IsRemoteZone() As Boolean
    Property RobotIndex() As Integer
    Property ScatteredAccessData() As String()
    Property LinkIndex() As ePLCLink
    Sub Initialize(Optional ByVal sParam As String = "") 'sParam will be specific to the sub-type, as needed
    Sub MakeCarMove()
    Sub subUpdateSAData()
    Overloads Sub Show(ByVal StartData As String())
    Sub PrivilegeChange(ByVal NewPrivilege As ePrivilege)
    Sub subCleanUpRobotLabels(ByVal rArm As clsArm)
    Sub InitPLCData()
    Sub UpdatePLCData(Optional ByVal pLink As ePLCLink = ePLCLink.None)
End Interface
