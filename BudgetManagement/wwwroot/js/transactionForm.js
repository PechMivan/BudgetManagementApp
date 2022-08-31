function getCategories(url) {
    $("#OperationTypeId").change(async function () {
        const OperationTypeValue = $(this).val();

        const response = await fetch(url, {
            method: "POST",
            body: OperationTypeValue,
            headers: {
                "Content-Type": "application/json"
            }
        });

        const jsonResponse = await response.json();
        const options = await jsonResponse.map(category => `<option value=${category.value}>${category.text}</option>`);
        $("#CategoryId").html(options);
    })
}
