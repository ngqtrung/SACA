//const tinymce = require("../lib/tinymce/tinymce");

let openModals = [];
let activeTinymceEditors = [];
const tinymceElements = ["tinymce-modal", "tinymce-modal-2"];

async function openDeleteModal(target, id, url, modalId, modalContainer = "modalContainer") {
    if (!id || !modalId) return;
    await openModal(`${url}?target=${target}&id=${id}`, modalId, modalContainer);
}

async function openEditModal(row, url, modalId, modalContainer = "modalContainer") {
    if (!row || !modalId) return;

    let item = Object.fromEntries([...row.attributes]
        .filter(attr => attr.name.startsWith("data-"))
        .map(attr => [attr.name.replace("data-", ""), attr.value])
    );

    const connector = url.includes('?') ? '&' : '?';
    const finalUrl = `${url}${connector}modalAction=Edit`;
    
    await openModal(finalUrl, modalId, modalContainer);

    const modal = document.querySelector(`#${modalId}`);
    if (!modal) return;

    Object.keys(item).forEach(key => {
        let input = modal.querySelector(`[name='${key}']`);
        if (input) {
            if (input.getAttribute("type") === "file") {
                input.setAttribute("data-value", item[key]); //set pseudo-value for input type="file"
            } else {
                input.value = item[key];
            }
        }
    });
}

async function openModal(url, modalId, modalContainer="modalContainer") {
    try {
        const response = await fetch(url);
        const data = await response.text();

        const tempDiv = document.createElement("div");
        tempDiv.innerHTML = data;

        const scripts = tempDiv.querySelectorAll("script");

        document.getElementById(modalContainer).innerHTML = tempDiv.innerHTML;
        document.getElementById(modalId).classList.remove("hidden");

        // Execute each script manually
        scripts.forEach(script => {
            if (script.src) {
                // If script has a src attribute (external script), dynamically load it
                const newScript = document.createElement("script");
                newScript.src = script.src;
                document.body.appendChild(newScript);
            } else {
                // If inline script, execute it
                eval(script.textContent);
            }
        });

        tinymceElements.forEach(ele => {
            if (document.querySelector(`#${modalId} #${ele}`)) {
                tinymce.init({
                    selector: `#${ele}`,
                    license_key: 'gpl',
                    toolbar: 'undo redo | formatselect | bold italic | image | alignleft aligncenter alignright | table',
                    plugins: 'table image',
                    image_uploadtab: true,
                    automatic_uploads: true,
                    //images_upload_url: '/ContestManagement/UploadFile',
                    file_picker_types: 'image',
                });
                activeTinymceEditors.push(ele);
            }
        })

        // Add modal to open stack
        openModals.push(modalId);

        if (openModals.length == 1) {
            document.body.style.overflow = "hidden";
        }
    } catch (error) {
        console.error("Error loading modal:", error);
    }
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.add("hidden");

    activeTinymceEditors = activeTinymceEditors.filter(editorId => {
        const editorElement = document.querySelector(`#${modalId} #${editorId}`);
        if (editorElement) {
            tinymce.get(editorId)?.remove();
            return false; // Remove from active list
        }
        return true; // Keep in active list
    });

    // Remove modal from open stack
    openModals = openModals.filter(id => id !== modalId);

    if (openModals.length == 0) {
        document.body.style.overflow = "auto";
    }
}