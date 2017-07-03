$(document).ready(function () {
    function Validate(rp) {
        var err = "";

        if (rp.Items.length == 0) {
            err = "Please select item/s.";
        }
        if (rp.CheckedBy == null) {
            err = "Please put checked by.";
        }

        return err;
    }

    $('#btnSubmit').click(function (event) {
        event.preventDefault();
      

        var rp = new Object();

        rp.Date = $('#txtDate').val();
        rp.VendorId = $('#VendorId').val();
        rp.CheckedBy = $('#CheckedBy').val();   
        var lstItem = new Array();
 
        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.ItemId = $this.data("item-id");
            item.UnitCost = $this.find(".txtUnitCost").val();
            item.Quantity = $this.find(".txtQuantity").val();
            item.Remarks = $this.find(".txtRemarks").val();

            lstItem.push(item);
        });
 
        rp.Items = lstItem;

        var err = Validate(rp);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/RequisitionPurchase/Save",
            type: "POST",
            data: JSON.stringify(rp),
            contentType: "application/json; charset=utf-8",
            success: function (r) {
               
                alert("Saved");
                window.location.href = "/RequisitionPurchase/Index";
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

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var itemId = $('#Items').val();

        var url = $(this).data('url') + '?itemId=' + itemId;
        $.get(url, function (data) {
            $('#tblItems').prepend(data);
        });
    });

    $(document).on('click', '.btnRemoveItem', function () {
        var $row = $(this).closest("tr");
        $row.fadeOut("fast", function () {
            $row.remove();
        })
    });


    $(document).on('keyup change', '.txtQuantity', function () {
        $tr = $(this).closest('tr');

        var itemId = $tr.data('item-id');
        var unitCost = RemoveCommas($tr.find('.txtUnitCost').val());
        var quantity = $tr.find('.txtQuantity').val();
        var amount = parseFloat(unitCost) * parseFloat(quantity);

        $txtAmount = $tr.find(".txtAmount");
        $txtAmount.text(FormatNumber(amount));

        GetTotalAmount();
    });

    $(document).on('keyup change', '.txtUnitCost', function () {
        $tr = $(this).closest('tr');

        var itemId = $tr.data('item-id');
        var unitCost = RemoveCommas($tr.find('.txtUnitCost').val());
        var quantity = $tr.find('.txtQuantity').val();
        var amount = parseFloat(unitCost) * parseFloat(quantity);

        $txtAmount = $tr.find(".txtAmount");
        $txtAmount.text(FormatNumber(amount));

        GetTotalAmount();
    });

    $('#txtDate').change(function () {
        var txtDate = $(this).val();
        $.ajax({
            url: "/RequisitionPurchase/GetRPNo",
            type: "POST",
            data: "{'Date' : '" + txtDate + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("#lblRpNo").html(data).show();
            }
        });
    });
});


function GetTotalAmount() {
    var totalAmount = 0;
    $('#tblItems tr.item-row').each(function (i, tr) {
        var $txtAmount = $(tr).find('.txtAmount');
        var amount = parseFloat(RemoveCommas($txtAmount.text()));

        totalAmount += amount;
    });

    $('#lblTotalAmount').text(FormatNumber(totalAmount));
}