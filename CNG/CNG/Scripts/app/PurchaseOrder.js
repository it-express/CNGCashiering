$(document).ready(function () {
    GetGeneratePoNumber();

    $('#Vendor').change(function () {
        var vendorId = $(this).val();

        $.ajax({
            url: "/Vendor/GetById",
            type: "POST",
            data: "{'id' : '" + vendorId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                var vendor = r;

                $('#lblVendorName').text(vendor.Name);
                $('#lblVendorAddress').text(vendor.Address);
                $('#lblVendorContactPerson').text(vendor.ContactPerson);
                $('#lblVendorContactNo').text(vendor.ContactNo);
            }
        });
    });

    $('#Company').change(function () {
        var companyId = $(this).val();

        $.ajax({
            url: "/Company/GetById",
            type: "POST",
            data: "{'id' : '" + companyId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                var company = r;

                $('#lblCompanyname').text(company.Name);
                $('#lblShippingAddress').text(company.Address);
                $('#lblShippingContactPerson').text(company.ContactPerson);
                $('#lblShippingContactNo').text(company.ContactNo);
            }
        });
    });

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var itemId = $('#Items').val();

        $.ajax({
            url: "/PurchaseOrder/AddItem",
            type: "POST",
            data: "{'itemId' : '" + itemId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                $('#tblItems tbody').empty();

                $.ajax({
                    url: "/PurchaseOrder/ListItems",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (r) {

                        var result = "";
                        $.each(r, function (i, v) {
                            result += "<tr>";
                            result += "<td>" + v.Code + "</td>";
                            result += "<td>" + v.Description + "</td>";
                            result += "<td>" + v.Brand + "</td>";
                            result += "<td>" + v.UnitCost + "</td>";
                            result += "<td>" + v.Type.Description + "</td>";
                            result += "</tr>";
                        });

                        $('#tblItems').append(result);
                    }
                });
            }
        });
    });
});

function GetGeneratePoNumber() {
    $.ajax({
        url: "/PurchaseOrder/GeneratePoNumber",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (r) {
            $('#lblPoNumber').text(r);
        }
    });
}