$vcenters = @("ptekvcs01", "apgaraavcs801", "apgartstvcs201", "ptekvcsd01", "apgartksvcs201", "apgartksvcs801")

$encrypted = Get-Content F:\Serhat\servicejobs_password.txt | ConvertTo-SecureString
$servicejobscred = New-Object System.Management.Automation.PsCredential("GARANTI\servicejobs", $encrypted)

$conn = New-Object System.Data.SqlClient.SqlConnection "Data Source=TEKSCR1\SQLEXPRESS;Database=CloudUnited;Integrated Security=True"
$conn.Open()


foreach ($vcenter in $vcenters) {
    Connect-VIServer -Server $vcenter -Credential $servicejobscred
    $clusterDataList = @()
        
    $clusters = Get-Cluster
    foreach ($cluster in $clusters) {
        $clusterName = $cluster.Name -replace '#', ''
        $datacenter = ($cluster | Get-Datacenter).Name
        $clusterHAE = $cluster.HAEnabled
        $clusterDrsEnabled = $cluster.DRSEnabled

        $clusterTotalCPU = "{0:F2}" -f [decimal]($cluster.ExtensionData.Summary.TotalCpu / 1000)
        $clusterTotalMemory = "{0:F2}" -f [decimal]($cluster.ExtensionData.Summary.TotalMemory / 1GB)

        $clusterNumVmotions = $cluster.ExtensionData.Summary.NumVmotions
        $clusterNumHosts = $cluster.ExtensionData.Summary.NumHosts
        $clusterNumCpuCores = $cluster.ExtensionData.Summary.NumCpuCores
        $totalVmCount = $cluster.ExtensionData.Summary.UsageSummary.TotalVmCount
        $lastWriteTime = Get-Date -Format "yyyy-MM-dd"

        # Gather datastore information
        $datastores = $cluster | Get-Datastore
        $dsData = $datastores | ForEach-Object {
            [PSCustomObject]@{
                Name        = $_.Name
                CapacityGB = $_.CapacityGB
                FreeSpaceGB = $_.FreeSpaceGB
                PercentUsed = [math]::Round(($_.CapacityGB - $_.FreeSpaceGB) / $_.CapacityGB * 100, 2)
            }
        }

        $dsNames = ($dsData | Select-Object -ExpandProperty Name) -join '~'
        $dsCapacity = ($dsData | ForEach-Object { "{0:F2}" -f $_.CapacityGB }) -join '~'
        $dsFree = ($dsData | ForEach-Object { "{0:F2}" -f $_.FreeSpaceGB }) -join '~'
        $dsPercentUsing = ($dsData | ForEach-Object { $_.PercentUsed }) -join '~'
        $totalStorage = ($dsData | Measure-Object -Property CapacityGB -Sum).Sum
        $freeStorage = ($dsData | Measure-Object -Property FreeSpaceGB -Sum).Sum

        $clusterDataList += @"
            ('$clusterName', '$datacenter', '$clusterHAE', '$vcenter', '$clusterDrsEnabled', 
             $clusterTotalMemory, $clusterTotalCPU, $clusterNumVmotions,
             $clusterNumHosts, $clusterNumCpuCores, $totalVmCount, '$lastWriteTime', 
             '$dsNames', '$dsCapacity', '$dsFree', '$dsPercentUsing', 
             $("{0:F2}" -f [decimal]($totalStorage)), $("{0:F2}" -f [decimal]($freeStorage)))
"@
    }
    
    if ($clusterDataList.Count -gt 0) {
        $values = $clusterDataList -join ","
        $sql = @"
            INSERT INTO vminfoClusters (
                ClusterName, DataCenter, ClusterHAE, vCenter, ClusterDRSEnabled, 
                ClusterTotalMemory, ClusterTotalCPU, ClusterNumVmotions,
                ClusterNumHosts, ClusterNumCPUCores, TotalVMCount, LastWriteTime, 
                DSName, DSCapacity, DSFree, DSPercentUsing, 
                ClusterTotalStorage, ClusterFreeStorage, VMHostNames) 
            VALUES $values
"@

        $command = $conn.CreateCommand()
        $command.CommandText = $sql
        $command.ExecuteNonQuery()
    }

       
    Disconnect-VIServer -Server $vcenter -Confirm:$false
}

$conn.Close()
