
//formId thường là modalId. Chủ yếu cái này là dùng cho Modal bởi vì nó là PartialView còn Page hay Layout chỉ cần gắn partial cho ValidationScriptsPartial là nó tự động validate rồi
function displayValidationErrors(formId, errors) { 
    const form = document.getElementById(formId);

    // Clear previous errors
    form.querySelectorAll(".text-red-500").forEach(el => el.remove());

    // Display new errors
    errors.forEach(errorObj => {
        const input = form.querySelector(`[name="${errorObj.key}"]`);
        if (input) {
            const errorDiv = document.createElement("div");
            errorDiv.className = "text-red-500 text-sm mt-1";
            errorDiv.innerText = errorObj.errors.join(", ");
            input.insertAdjacentElement("afterend", errorDiv);
        }
    });
}