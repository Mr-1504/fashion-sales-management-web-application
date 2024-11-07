$(document).ready(function () {
    $(document).on('click', '.js-btn-plus', function () {
        let input = $(this).closest('tr').find('input:eq(1)');
        let value = parseInt(input.val());
        var row = input.closest('tr')
        updateTotal(row);
        updateItemQuantity(parseInt(row.find('input:eq(0)').val()), value);
    });

    $(document).on('click', '.js-btn-minus', function () {
        let input = $(this).closest('tr').find('input:eq(1)');
        let value = parseInt(input.val());
        var row = input.closest('tr')
        if (value > 0) {
            updateTotal(row);
            updateItemQuantity(parseInt(row.find('input:eq(0)').val()), value);
        } else {
            input.val(value + 1);
        }
    });
});
$(document).on('change', '#all', function () {
    let isChecked = $(this).is(':checked');
    $('.site-blocks-table tbody tr').each(function () {
        $(this).find('.product-checkbox').prop('checked', isChecked);
    });
    updateTotalAmount();
});
$(document).on('change', '.product-checkbox', function () {
    let isCheckAll = true;
    $('.site-blocks-table tbody tr').each(function () {
        if (!$(this).find('.product-checkbox').prop('checked')) {
            isCheckAll = false;
        }
    });

    $('#all').prop('checked', isCheckAll);
    updateTotalAmount();
});

$('.quantity').on('change', function () {
    var row = $(this).closest('tr');
    var itemId = row.data('id');
    var newQuantity = $(this).val();

    updateItemQuantity(itemId, newQuantity);
});

var debounceTimer;
function updateItemQuantity(Id, Quantity) {
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(function () {
        $.ajax({
            type: "PATCH",
            url: "/api/cartapi/update",
            data: JSON.stringify({ Id, Quantity }),
            contentType: "application/json;",
            success: function (response) {
            },
            error: function () {
                alert("Cập nhật không thành công.");
            }
        });
    }, 600);
}

function updateTotalAmount() {
    let totalAmount = 0;
    $('.product-checkbox:checked').each(function () {
        let row = $(this).closest('tr');
        let price = parseFloat(row.find('td:eq(5)').text());
        let quantity = parseInt(row.find('input:eq(1)').val());
        totalAmount += price * quantity;
    });

    $('.total-amount').text(totalAmount);
}

function updateTotal(row) {
    let price = parseFloat(row.find('td:eq(5)').text());
    let quantity = parseInt(row.find('input:eq(1)').val());
    let total = price * quantity;
    row.find('td:eq(7)').text(total);

    let grandTotal = 0;
    $('.site-blocks-table tbody tr').each(function () {
        if ($(this).find('#check').prop('checked')) {
            let rowTotal = parseFloat($(this).find('td:eq(7)').text()) || 0;
            grandTotal += rowTotal;
        }
    });
    $('.total-amount').text(grandTotal);
}
$(document).on('click', '#remove', function () {
    let row = $(this).closest('tr');
    let cartId = parseInt(row.find('input:eq(0)').val());
    $.ajax({
        url: '/api/cartapi/remove',
        type: 'DELETE',
        contentType: 'application/json',
        data: JSON.stringify(cartId),
        success: function (response) {
            updateCart();
        },
        error: function (xhr, status, error) {
            alert(xhr.status + " " + cartId);
        }
    });
});

function updateCart() {

    $.ajax({
        url: `/items`,
        type: 'GET',
        success: function (response) {
            $('#items').html(response);
            var count = parseInt($('.count').text());
            $('.count').show();
            $('.count').text(count - 1);
        },
        error: function (xhr, status, error) {
            $('#cart-container').html('<h2>Không có sản phẩm nào trong giỏ hàng</h2>')
            $('.count').hide();
        }
    });
}