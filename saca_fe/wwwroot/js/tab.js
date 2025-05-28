async function fetchPaginatedTabs(url, data, tab_id, page_index = 1, page_size = 5, is_drafted = false, is_view_only = false, contest_id="") {
    try {
        const response = await fetch(`${url}?page_index=${page_index}&page_size=${page_size}&is_drafted=${is_drafted}&is_view_only=${is_view_only}&contest_id=${contest_id}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data),
            redirect: "manual"
        });

        if (!response.ok) throw new Error("Failed to fetch data");

        const respText = await response.text();

        const tempDiv = document.createElement("div");
        tempDiv.innerHTML = respText;
        const scripts = tempDiv.querySelectorAll("script");
        document.getElementById(tab_id).innerHTML = tempDiv.innerHTML;
        
        scripts.forEach(script => {
            if (script.src) {
                const newScript = document.createElement("script");
                newScript.src = script.src;
                document.body.appendChild(newScript);
            } else {
                eval(script.textContent);
            }
        });
    } catch (error) {
        console.error("Error fetching tabs:", error);
    }
}