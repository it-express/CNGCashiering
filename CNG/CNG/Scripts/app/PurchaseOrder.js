$(document).ready(function () {
    $(document).on('click', '.btnRemoveItem', function () {
        var $row = $(this).closest("tr");
        $row.fadeOut("fast", function () {
            $row.remove();

            GetTotalAmount();
        })
    });

    $(document).on('keyup change', '.txtQuantity', function () {
        $tr = $(this).closest('tr');

        var itemId = $tr.data('item-id');
        var unitCost = RemoveCommas($tr.find('.lblUnitCost').val());
        var quantity = $tr.find('.txtQuantity').val();
        var amount = parseFloat(unitCost) * parseFloat(quantity);

        $txtAmount = $tr.find(".txtAmount");
        $txtAmount.text(FormatNumber(amount));

        GetTotalAmount();
    });

    $(document).on('keyup change', '.lblUnitCost', function () {
        $tr = $(this).closest('tr');

        var itemId = $tr.data('item-id');
        var unitCost = RemoveCommas($tr.find('.lblUnitCost').val());
        var quantity = $tr.find('.txtQuantity').val();
        var amount = parseFloat(unitCost) * parseFloat(quantity);
      
        $txtAmount = $tr.find(".txtAmount");
        $txtAmount.text(FormatNumber(amount));

        GetTotalAmount();
    });

    $('#btnSubmit').click(function (event) {
        event.preventDefault();
        
        var purchaseOrder = new Object();
        purchaseOrder.No = $('#lblPoNumber').text();
        purchaseOrder.Date = $('#txtDate').val();
        purchaseOrder.VendorId = $('#VendorId').val();
        purchaseOrder.ShipTo = $('#ShipTo').val();
        //purchaseOrder.Terms = (backen generated)
        //purchaseOrder.PreparedBy = (backend generated);
        //purchaseOrder.ApprovedBy = (backend generated);
        purchaseOrder.CheckedBy = $('#CheckedBy').val();

        purchaseOrder.Items = GetSelectedItems();

        var err = Validate(purchaseOrder);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/PurchaseOrder/Save",
            type: "POST",
            data: JSON.stringify(purchaseOrder),
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                alert("Successfully saved Purchase Order");

                var companyId = getParameterByName("companyId");
                window.location.href = "/PurchaseOrder/Index?companyId=" + companyId;
            }
        });
    });

    $('#VendorId').change(function () {
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

                $('#lblTerms').text(vendor.Terms + ' days');

                var today = new Date();
                var dueDate = moment(today).add('days', vendor.Terms);

                $('#lblDueDate').text(dueDate.format("MMMM Do YYYY"));
            }
        });
    });

    $('#ShipTo').change(function () {
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

        var url = $(this).data('url') + '?itemId=' + itemId;
        $.get(url, function (data) {
            $('#tblItems').prepend(data);
        });
    });
});

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

function GetSelectedItems() {
    var lstItem = new Array();

    $("tr.item-row").each(function () {
        $this = $(this);

        var item = new Object();
        item.Id = $this.data("item-id");
        item.Quantity = $this.find(".txtQuantity").val();
        item.Remarks = $this.find(".txtRemarks").val();
        item.UnitCost = $this.find(".lblUnitCost").val();
        lstItem.push(item);
    });
    return lstItem;

    
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function Validate(po) {
    var err = "";

    if (po.VendorId == "") {
        err = "Vendor is required.";
    }
    else if (po.ShipTo == "") {
        err = "Ship To is required.";
    }
    else if (po.Items.length == 0) {
        err = "Please select item/s.";
    }

    return err;
}

function GetTotalAmount() {
    var totalAmount = 0;
    $('#tblItems tr.item-row').each(function (i,tr) {
        var $txtAmount = $(tr).find('.txtAmount');
        var amount = parseFloat(RemoveCommas($txtAmount.text()));

        totalAmount += amount;
    });

    $('#lblTotalAmount').text(FormatNumber(totalAmount));
}