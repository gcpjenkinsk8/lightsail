
function getBoolFromString(e) {
    if (e === 'False')
        return false;


    return true;
}

function setGridEditMode(canUpdate) {
    $(".k-grid-add").kendoButton({ enable: canUpdate })
        .data("kendoButton").enable(canUpdate);
    $(".k-grid-save-changes").kendoButton({ enable: canUpdate })
        .data("kendoButton").enable(canUpdate);
    $(".k-grid-cancel-changes").kendoButton({ enable: canUpdate })
        .data("kendoButton").enable(canUpdate);
}

function onSort(e) {
    var gridData = e.sender.dataSource.data()
    gridData.forEach(function (element) {
        if (!element.ProductName) {
            e.preventDefault()
        }
    });
}

function reloadData(e) {
    //if (e.type === 'create') {
    //    if (!e.response.Errors) {
    //        e.sender.read();
    //    }
    //}

}
function error_handler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
    }
}


function getFileInfo(e) {
    return $.map(e.files, function (file) {
        var info = file.name;

        // File size is not available in all browsers
        if (file.size > 0) {
            info += " (" + Math.ceil(file.size / 1024) + " KB)";
        }
        return info;
    }).join(", ");
}



function onSelect(e) {
    //kendoConsole.log("Select :: " + getFileInfo(e));
}

function onUpload(e) {
    //kendoConsole.log("Upload :: " + getFileInfo(e));
}

function onSuccess(e) {
    //kendoConsole.log("Success (" + e.operation + ") :: " + getFileInfo(e));
}

function onError(e) {
    alert(e.XMLHttpRequest.response);

}

function onComplete(e) {


}

function onCancel(e) {
    //kendoConsole.log("Cancel :: " + getFileInfo(e));
}

function onRemove(e) {
    //kendoConsole.log("Remove :: " + getFileInfo(e));
}

function onProgress(e) {
    //kendoConsole.log("Upload progress :: " + e.percentComplete + "% :: " + getFileInfo(e));
}

function onClear(e) {
    //kendoConsole.log("Clear");
}



