#include <QmlNet/qml/NetVariantList.h>

NetVariantList::NetVariantList() = default;

NetVariantList::~NetVariantList() = default;

int NetVariantList::count() {
    return variants.size();
}

void NetVariantList::add(const QSharedPointer<NetVariant>& variant) {
    variants.append(variant);
}

QSharedPointer<NetVariant> NetVariantList::get(int index) {
    if(index < 0) return QSharedPointer<NetVariant>(nullptr);
    if(index >= variants.length()) return QSharedPointer<NetVariant>(nullptr);
    return variants.at(index);
}

void NetVariantList::remove(int index) {
    variants.removeAt(index);;
}

void NetVariantList::clear() {
    variants.clear();
}

QString NetVariantList::debugDisplay()
{
    QString result;
    for(int x = 0; x < variants.length(); x++) {
        result.append(variants.at(x)->getDisplayValue());
        if(x < variants.length() - 1) {
            result.append(", ");
        }
    }
    return result;
}

extern "C" {

Q_DECL_EXPORT NetVariantListContainer* net_variant_list_create() {
    NetVariantListContainer* result = new NetVariantListContainer();
    result->list = QSharedPointer<NetVariantList>(new NetVariantList());
    return result;
}

Q_DECL_EXPORT void net_variant_list_destroy(NetVariantListContainer* container) {
    delete container;
}

Q_DECL_EXPORT int net_variant_list_count(NetVariantListContainer* container) {
    return container->list->count();
}

Q_DECL_EXPORT void net_variant_list_add(NetVariantListContainer* container, NetVariantContainer* variant) {
    container->list->add(variant->variant);
}

Q_DECL_EXPORT NetVariantContainer* net_variant_list_get(NetVariantListContainer* container, int index){
    QSharedPointer<NetVariant> variant = container->list->get(index);
    if(variant == nullptr) return nullptr;
    NetVariantContainer* result = new NetVariantContainer();
    result->variant = variant;
    return result;
}

Q_DECL_EXPORT void net_variant_list_remove(NetVariantListContainer* container, int index) {
    container->list->remove(index);
}

Q_DECL_EXPORT void net_variant_list_clear(NetVariantListContainer* container) {
    container->list->clear();
}

}
