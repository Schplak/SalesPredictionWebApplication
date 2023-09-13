function dropHandler(ev) {
    console.log("File(s) dropped");

    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();

    var filenamesList = [];
    var formData = new FormData();

    if (ev.dataTransfer.items) {

        filenamesList = document.getElementById("filename_list");

        // Use DataTransferItemList interface to access the file(s)
        [...ev.dataTransfer.items].forEach((item, i) => {
            // If dropped items aren't files, reject them
            if (item.kind === "file") {
                const file = item.getAsFile();
                console.log(`… file[${i}].name = ${file.name}`);

                var li = document.createElement('li');
                li.innerText = file.name;
                filenamesList.appendChild(li);
                //formData.append(file.name);
            }
        });
    } else {
        // Use DataTransfer interface to access the file(s)
        [...ev.dataTransfer.files].forEach((file, i) => {
            console.log(`… file[${i}].name = ${file.name}`);

            var li = document.createElement('li');
            li.innerText = file.name;
            filenamesList.appendChild(li);
            //formData.append(file);
        });
    }

    if (filenamesList.children.length > 0) {
        document.getElementById("drop_zone_filename_text").innerHTML = "CSV files added:";

    }
}

function dragOverHandler(ev) {
    console.log("File(s) in drop zone");

    ev.target.style.border = '2px dashed red';
    ev.target.style.background = '#f1f1f1';

    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();
}

function dragLeaveHandler(ev) {
    console.log("File(s) left drop zone");

    ev.target.style.border = '2px dashed blue';
    ev.target.style.background = '#ffffff';

    // Prevent default behavior (Prevent file from being opened)
    ev.preventDefault();
}


function displayNames(input, target) {
    const output = document.getElementById(target);
    output.innerHTML = '';
    const files = input.files
    for (let i = 0; i < files.length; i++) {
        let file = files.item(i)
        let el = document.createElement('li');
        el.innerText = file.name;
        output.appendChild(el);
    }
}