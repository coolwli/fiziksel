var relocateSucceeded = false;

// ===============================
// 1. ÖN KONTROLLER
// ===============================

if (vm.runtime.powerState != VcVirtualMachinePowerState.poweredOff) {
    throw "VM poweredOff olmalı.";
}

System.log("VM hazır: " + vm.name);

// ===============================
// 2. MEVCUT DURUM YEDEĞİ
// ===============================

var originalDiskMap = new Properties(); // label → datastore

for each (var dev in vm.config.hardware.device) {
    if (dev instanceof VcVirtualDisk) {
        originalDiskMap.put(
            dev.deviceInfo.label,
            dev.backing.datastore.name
        );
    }
}

System.log("Orijinal disk durumu kaydedildi.");

// ===============================
// 3. RELOCATE SPEC
// ===============================

var relocateSpec = new VcVirtualMachineRelocateSpec();
relocateSpec.disk = [];

for each (var dev in vm.config.hardware.device) {

    if (!(dev instanceof VcVirtualDisk)) continue;

    var label = dev.deviceInfo.label;
    var targetDsName = diskToDatastoreMap.get(label);

    if (!targetDsName) continue;

    var targetDs = null;
    for each (var ds in vm.datastore) {
        if (ds.name == targetDsName) {
            targetDs = ds;
            break;
        }
    }

    if (!targetDs) {
        throw "Datastore resolve edilemedi: " + targetDsName;
    }

    var locator = new VcVirtualMachineRelocateSpecDiskLocator();
    locator.diskId = dev.key;
    locator.datastore = targetDs.reference;

    relocateSpec.disk.push(locator);
}

if (relocateSpec.disk.length === 0) {
    throw "Taşınacak disk bulunamadı.";
}

// ===============================
// 4. RELOCATE
// ===============================

try {
    System.log("Relocate başlatılıyor...");
    var task = vm.relocateVM_Task(relocateSpec);

    System.getModule("com.vmware.library.vc.basic")
          .vim3WaitTaskEnd(task, true, 0);

    relocateSucceeded = true;
    System.log("Relocate tamamlandı.");

} catch (e) {
    System.error("Relocate hatası: " + e);
    rollback(vm, originalDiskMap);
    throw "Relocate başarısız, rollback yapıldı.";
}

// ===============================
// 5. VALIDATION
// ===============================

try {
    System.log("Validation başlatılıyor...");

    for each (var dev in vm.config.hardware.device) {
        if (!(dev instanceof VcVirtualDisk)) continue;

        var expectedDs = diskToDatastoreMap.get(dev.deviceInfo.label);
        if (!expectedDs) continue;

        var actualDs = dev.backing.datastore.name;

        if (actualDs != expectedDs) {
            throw dev.deviceInfo.label +
                  " yanlış datastore’da: " + actualDs;
        }
    }

    System.log("✔ Validation başarılı.");

} catch (vErr) {

    System.error("VALIDATION FAIL: " + vErr);

    if (relocateSucceeded) {
        rollback(vm, originalDiskMap);
        throw "Validation başarısız → rollback yapıldı.";
    }

    throw vErr;
}

// ===============================
// 6. ROLLBACK FONKSİYONU
// ===============================

function rollback(vm, originalDiskMap) {

    System.warn("ROLLBACK başlatılıyor...");

    var rbSpec = new VcVirtualMachineRelocateSpec();
    rbSpec.disk = [];

    for each (var dev in vm.config.hardware.device) {

        if (!(dev instanceof VcVirtualDisk)) continue;

        var oldDsName = originalDiskMap.get(dev.deviceInfo.label);
        if (!oldDsName) continue;

        var oldDs = null;
        for each (var ds in vm.datastore) {
            if (ds.name == oldDsName) {
                oldDs = ds;
                break;
            }
        }

        if (!oldDs) {
            System.error("Rollback datastore bulunamadı: " + oldDsName);
            continue;
        }

        var rbLocator = new VcVirtualMachineRelocateSpecDiskLocator();
        rbLocator.diskId = dev.key;
        rbLocator.datastore = oldDs.reference;

        rbSpec.disk.push(rbLocator);
    }

    if (rbSpec.disk.length === 0) {
        System.error("Rollback için disk bulunamadı.");
        return;
    }

    var rbTask = vm.relocateVM_Task(rbSpec);

    System.getModule("com.vmware.library.vc.basic")
          .vim3WaitTaskEnd(rbTask, true, 0);

    System.warn("ROLLBACK tamamlandı.");
}
